#!/bin/bash

cd /C/Program\ Files/Docker\ Toolbox

trap '[ "$?" -eq 0 ] || read -p "Looks like something went wrong... Press any key to continue..."' EXIT

VM=default
DOCKER_MACHINE=./docker-machine.exe

if [ ! -z "$VBOX_MSI_INSTALL_PATH" ]; then
  VBOXMANAGE=${VBOX_MSI_INSTALL_PATH}VBoxManage.exe
else
  VBOXMANAGE=${VBOX_INSTALL_PATH}VBoxManage.exe
fi

BLUE='\033[1;34m'
GREEN='\033[0;32m'
NC='\033[0m'

if [ ! -f $DOCKER_MACHINE ] || [ ! -f "${VBOXMANAGE}" ]; then
  echo "Either VirtualBox or Docker Machine are not installed. Please re-run the Toolbox Installer and try again."
  exit 1
fi

"${VBOXMANAGE}" startvm "$VM" --type headless

#"${VBOXMANAGE}" showvminfo $VM &> /dev/null
#VM_EXISTS_CODE=$?

#set -e

#if [ $VM_EXISTS_CODE -eq 1 ]; then
#  echo "Creating Machine $VM..."
#  $DOCKER_MACHINE rm -f $VM &> /dev/null || :
#  rm -rf ~/.docker/machine/machines/$VM
#  $DOCKER_MACHINE create -d virtualbox --virtualbox-memory 2048 $VM
#else
#  echo "Machine $VM already exists in VirtualBox."
#fi