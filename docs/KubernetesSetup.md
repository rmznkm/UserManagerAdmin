${SolutionPath} => Projenin bulunduğu path
helm kurulumu öncesinde yapılması gerekenler
	helm repo add bitnami https://charts.bitnami.com/bitnami
	helm repo add nginx-stable https://helm.nginx.com/stable

1-) Projenin bulunduğu pathe gidilip imagelar image repository ye atılır.
	cd  ${SolutionPath}
	docker build -t rmzn/userapi-image:v1 --file src/UserAPI/Dockerfile .
	docker build -t rmzn/managementapi-image:v1 --file src/ManagementAPI/Dockerfile .

2-) RabbitMQ kurulumu
	a-) Kurulum
		helm install my-rabbit --set auth.username=admin,auth.password=admin,auth.erlangCookie=secretcookie bitnami/rabbitmq
		//nginx controller kullanılmıcaksa opsiyonel
		kubectl port-forward --namespace default svc/my-rabbit-rabbitmq 15672:15672
		kubectl port-forward --namespace default svc/my-rabbit-rabbitmq 5672:5672
	b-) Kaldırma
		helm delete my-rabbit

3-) Postgres kurulumu
	a-) Kurulum
		helm install my-postgresdeneme  bitnami/postgresql \
		--set global.postgresql.auth.postgresPassword=postgres \
		--set auth.postgresPassword=postgres \
		--set global.postgresql.auth.database=postgres \
		--set primary.initdb.scripts."init\.sql"="CREATE TABLE IF NOT EXISTS userapprovalrequest (id uuid primary key\, first_name character varying(255) NOT NULL\, last_name character varying(255) NOT NULL\, updated_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL\, record_status \"char\"); CREATE INDEX CONCURRENTLY record_status_index on userapprovalrequest USING HASH (record_status);" 
		
		// opsiyonel gerek yok
		kubectl port-forward --namespace default svc/my-postgresdeneme-postgresql 5432:5432

	b-) Kaldırma
		helm delete my-postgresdeneme

4-) UserApi nin deploment ve service uygulama
	cd  ${SolutionPath}\build\UserAPI
	kubectl apply -f deploment.yml
	kubectl apply -f service.yml

5-) ManagementApi nin deploment ve service uygulama
	cd  ${SolutionPath}\build\ManagementAPI
	kubectl apply -f deploment.yml
	kubectl apply -f service.yml

6-) Nginx kurulumu
	helm install app-ingress nginx-stable/nginx-ingress  --set controller.replicaCount=1

7-) RabbitMQ, ManagementAPI, UserAPI ingress uygulama
    sertifika create etme => openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout tls.key -out tls.crt -subj "//CN=management.myexample.com//O=management.myexample.com"
	cd  ${SolutionPath}\build
	kubectl apply -f rabbitmq-ingress.yml
	kubectl apply -f user-ingress.yml
	kubectl apply -f management-ingress.yml

	Bu işlemden sonra subdomain ingress kullanldığı için etc/host dosyasına eklenmesi gereken
	 kubectl get service app-ingress-ingress-nginx-controller bu komutta gelen EXTERNAL-IP alınıp
		{EXTERNAL-IP} user.myexample.com
		{EXTERNAL-IP} management.myexample.com
		{EXTERNAL-IP} rabbitmq.myexample.com 

		Örn:
		127.0.0.1 user.myexample.com
		127.0.0.1 management.myexample.com
		127.0.0.1 rabbitmq.myexample.com 

8-) Not olarak ManagementAPI GRPC çalıştığı için management-ingress.yml daki certifika kubernetes tarafından validate edilebilir.
	Bunu aşmak için geçici seçenek olarak aşağıdaki komut ile 7000 portundan çalışması sağlanabilir.
	kubectl port-forward --namespace default svc/managementapi-service 7000:80

9-) Uygulama pathleri

	-UserApi http-path http://user.myexample.com
	
	-ManagementApi grpc-path 
		ingress ->  management.myexample.com 
		port-forward -> localhost:7000