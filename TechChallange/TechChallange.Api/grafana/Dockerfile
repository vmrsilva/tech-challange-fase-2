FROM grafana/grafana:latest

# Define a porta padrão do Grafana
EXPOSE 3000

# Copia um arquivo de configuração personalizado (opcional)
# COPY grafana.ini /etc/grafana/grafana.ini

# Define variáveis de ambiente para configuração (opcional)
 ENV GF_SECURITY_ADMIN_USER=admin
 ENV GF_SECURITY_ADMIN_PASSWORD=admin

# Comando de inicialização
CMD ["/run.sh"]
