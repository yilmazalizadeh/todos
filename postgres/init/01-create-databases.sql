SELECT 'CREATE DATABASE travelapp_dev'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'travelapp_dev')\gexec

SELECT 'CREATE DATABASE travelapp_staging'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'travelapp_staging')\gexec

SELECT 'CREATE DATABASE travelapp_prod'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'travelapp_prod')\gexec
