# Contract Manager

This mod has as a goal to provide an easy way to create, manage and complete contracts in KSA.
It allows players to take on various contracts, complete objectives, and earn rewards.
For modders, it provides a flexible API to create custom contracts and objectives.

Finally, the end-goals of this mod are:
- To create an intuitive and user-friendly interface for managing contracts.
- To create a variety of community-driven contracts to keep game-play engaging and guide players beyond sending a rocket to space.
- To create a campaign mode where players can take on a series of contracts with increasing difficulty and complexity.

### Currently implemented features:
- Contract Manager Window to show offered, accepted and finished contracts and their details.
- Contract Manager Window to create and modify missions and contracts.
- Active Contracts Window to show the tracked requirements for the accepted contracts to achieve.
- Save/Load contract manager state with in-game save/load feature.
- Additional contracts can be created by adding them to the contracts folder

### Incomplete features:
- Created missions and contracts in-game cannot be saved/loaded nor exported. Expect that in the next release.
- Contract blueprints have an incomplete set of features for prerequisites, requirements and actions.
- Very simplistic example contracts.
- Loading contracts from other mods.

### Far future features/ideas:
- Multiplayer support for cooperative and competitive contract completion.
- Have (multi-)player and AI-controlled space agencies that can compete, accept and complete contracts. (space race!!!)
- Have non-space agencies that will offer commercial contracts that can influence the KSA economy and reputation system.

**Additional ideas and suggestions are welcome!**

## Dependencies

- [StarMap v0.3.6](https://github.com/StarMapLoader/StarMap/releases/tag/0.3.6)

## How to install

Follow the [StarMap](https://github.com/StarMapLoader/StarMap?tab=readme-ov-file#installation) installation instructions.

## API References:
* [Contracts](docs/Contract.md)

## Credits

First of all, thanks to [jrossignol](https://github.com/jrossignol) and contributors for creating and maintaining [`ContractConfigurator`](https://github.com/jrossignol/ContractConfigurator) for KSP. It has been a really awesome mod, and motivited and inspired me to create this mod for KSA.

Thanks to Arilynsky for reaching out and sharing ideas about creating missions and contracts in-game.

Thanks, to the KSA community for using this mod, their feedback and suggestions. I hope this mod will bring joy to playing KSA.
