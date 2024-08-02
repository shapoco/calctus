set /P CALCTUS_VERSION=<doc_version.txt
wsl make -f doc.mk PORT=%PORT% CALCTUS_VERSION=%CALCTUS_VERSION% all
