#!/bin/zsh

SCRIPT_PATH=$(realpath "$0" | sed 's|\(.*\)/.*|\1|')
echo "$SCRIPT_PATH"
cd "$SCRIPT_PATH/.." || exit
docker compose -f docker-compose.Production.yml build --no-cache || exit
mkdir -p ~/images
docker save -o ~/images/osu-czechia-bot.Production."$(date +%Y-%m-%d_%H-%M-%S)".tgz osu-czechia-bot