<p align="center">
  <img src="https://github.com/dalibor-osu/osu-czechia-bot/blob/main/osuczechia.png?raw=true" alt="osu! Czechia logo" width="25%"/>
</p>

# osu! Czechia Bot
Welcome to osu! Czechia bot repository. This is a Discord bot made for [osu! Czechia Discord server](https://discord.gg/RbwkhH5xS7). At the moment, it provides simple user authentication via osu! API, but more functionality is coming later.

## How to use
At the moment, there are 2 slash commands you can use once you join the server
- `/authorize`
  - Used for authorizing via osu! API. This command generates a link unique for your Discord user and sends it to you. Upon clicking on the link, you are redirected to osu! auth webpage, where you can either allow or decline the authorization. If you accept, you will be redirected to a osu! Czechia authorization page that handles everything else and closes itself if everything goes right.
- `/unlink`
  - Used for unlinking osu! profile that was linked to your Discord account using the previous `/authorize` command

## Contributing

Contributions are welcomed, however, no setup or contribution guide exists, so you will have to setup everything yourself. You can use docker compose and appsettings example files to help you with that.
