set /P CALCTUS_VERSION=<doc_version.txt
wsl make -f doc.mk CALCTUS_VERSION=%CALCTUS_VERSION% clean
