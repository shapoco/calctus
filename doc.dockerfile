FROM sphinxdoc/sphinx

RUN set -eux; \
    pip install sphinx-rtd-theme
