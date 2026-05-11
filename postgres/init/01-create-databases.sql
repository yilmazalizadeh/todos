SELECT 'CREATE DATABASE todoservice_dev'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'todoservice_dev')\gexec

SELECT 'CREATE DATABASE todoservice_staging'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'todoservice_staging')\gexec

SELECT 'CREATE DATABASE todoservice_prod'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'todoservice_prod')\gexec
