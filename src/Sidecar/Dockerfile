FROM envoyproxy/envoy:v1.27-latest

COPY Sidecar/envoy.yaml /config/envoy.yaml
COPY Sidecar/entrypoint.sh /
RUN chmod 500 /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]