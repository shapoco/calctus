set /P CALCTUS_VERSION=<doc_version.txt
set PORT=8080
wsl make -f doc.mk CALCTUS_VERSION=%CALCTUS_VERSION% all
start http://localhost:%PORT%/%CALCTUS_VERSION%/
wsl make -f doc.mk CALCTUS_VERSION=%CALCTUS_VERSION% PORT=%PORT% test
