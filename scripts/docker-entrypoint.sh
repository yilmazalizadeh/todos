#!/bin/sh
set -eu

if [ -n "${VAULT_ADDR:-}" ] && [ -n "${VAULT_TOKEN:-}" ]; then
    vault_environment="${VAULT_ENVIRONMENT:-dev}"
    vault_secret_path="${VAULT_SECRET_PATH:-secret/data/todoservice/${vault_environment}}"

    echo "Loading application configuration from Vault path ${vault_secret_path}..."

    connection_string=""
    attempt=1

    while [ "$attempt" -le 30 ]; do
        connection_string="$(
                curl -fsS \
                --header "X-Vault-Token: ${VAULT_TOKEN}" \
                "${VAULT_ADDR}/v1/${vault_secret_path}" \
                | jq -r '.data.data["ConnectionStrings__TodoDb"] // empty'
        )" && [ -n "$connection_string" ] && break

        echo "Vault secret is not available yet. Retrying in 2 seconds..."
        attempt=$((attempt + 1))
        sleep 2
    done

    if [ -z "$connection_string" ]; then
        echo "Failed to load ConnectionStrings__TodoDb from Vault." >&2
        exit 1
    fi

    export ConnectionStrings__TodoDb="$connection_string"
fi

exec "$@"
