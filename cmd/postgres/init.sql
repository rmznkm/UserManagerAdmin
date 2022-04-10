CREATE TABLE IF NOT EXISTS userapprovalrequest (
    id uuid primary key,
    first_name character varying(255) NOT NULL,
    last_name character varying(255) NOT NULL,
    updated_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
	record_status "char"
);

CREATE INDEX CONCURRENTLY record_status_index on userapprovalrequest USING HASH (record_status);