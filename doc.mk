.PHONY: all docker_build reset_project clean

WORK_DIR = $(shell pwd)
CALCTUS_VERSION := 0.7
CONTAINER_NAME = calctus-sphinx

PROJECT_DIR = ./docsrc
DOCS_DIR = ./docs
OUTPUT_DIR = $(DOCS_DIR)/$(CALCTUS_VERSION)
INDEX_HTML = $(OUTPUT_DIR)/index.html
DEPENDENCY_LIST = \
	doc.mk \
	$(wildcard $(PROJECT_DIR)/*.*) \
	$(wildcard $(PROJECT_DIR)/source/*.*) \
	$(wildcard $(PROJECT_DIR)/source/_images/*.*) \
	$(wildcard $(PROJECT_DIR)/source/_static/*.*) \
	$(wildcard $(PROJECT_DIR)/source/_templates/*.*)
HTTP_PORT := 8080

all: $(INDEX_HTML)

$(INDEX_HTML): $(DEPENDENCY_LIST)
	docker run \
		--rm \
		--user $(shell id -u):$(shell id -g) \
		-v $(WORK_DIR)/$(PROJECT_DIR):/docs \
		$(CONTAINER_NAME) \
		make html
	rm -rf $(OUTPUT_DIR)
	mkdir -p $(OUTPUT_DIR)
	cp -rp $(PROJECT_DIR)/build/html/* $(OUTPUT_DIR)
	find $(OUTPUT_DIR) -exec touch {} +

docker_build:
	docker build . -f doc.dockerfile -t $(CONTAINER_NAME)

reset_project:
	rm -rf $(PROJECT_DIR)
	mkdir -p $(PROJECT_DIR)
	docker run \
		-it \
		--rm \
		--user $(shell id -u):$(shell id -g) \
		-v $(WORK_DIR)/$(PROJECT_DIR):/docs \
		sphinxdoc/sphinx \
		sphinx-quickstart

test: $(INDEX_HTML)
	killall python3 || true
	@echo http://localhost:$(HTTP_PORT)/$(CALCTUS_VERSION)/
	python3 -m http.server $(HTTP_PORT) --directory $(DOCS_DIR)

clean:
	rm -rf $(PROJECT_DIR)/build/html
	rm -rf $(OUTPUT_DIR)
