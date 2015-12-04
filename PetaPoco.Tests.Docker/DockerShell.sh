#!/bin/bash

trap '[ "$?" -eq 0 ] || read -p "Looks like something went wrong... Press any key to continue..."' EXIT

VM=default
DOCKER_MACHINE=docker-machine.exe

echo "Setting environment variables for machine $VM..."
eval "$(${DOCKER_MACHINE} env --shell=bash $VM)"

clear
cat << EOF


                        ##         .
                  ## ## ##        ==
               ## ## ## ## ##    ===
           /"""""""""""""""""\___/ ===
      ~~~ {~~ ~~~~ ~~~ ~~~~ ~~~ ~ /  ===- ~~~
           \______ o           __/
             \    \         __/
              \____\_______/

EOF
echo -e "${BLUE}docker${NC} is configured to use the ${GREEN}$VM${NC} machine with IP ${GREEN}$($DOCKER_MACHINE ip $VM)${NC}"
echo "For help getting started, check out the docs at https://docs.docker.com"
echo "NOTE: When using interactive commands, prepend winpty. Examples: 'winpty docker run -it ...', 'winpty docker exec -it ...'."
echo

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd ${DIR}

exec "$BASH" --login -i
