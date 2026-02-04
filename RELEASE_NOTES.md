# Release Notes

## Version 0.3.0 2026-02-04 - Mission Planner

This release adds a mission planner window, that will allow the player to create missions that contain contracts.  
Missions can also be predefined and used to allow (other mods) to create a set of contracts (e.g. a contract pack).   
Currently, this mod offers an example mission mission to fly to Luna and back.

Furthermore, background migration was added to save file, and the contract/mission files. This will make any changes to the stored data future proof.

Supported KSA versions:
- `v2026.2.2.3396` Fully supported
- previous versions untested, but there was a change between `v2025.12.31.3103` and `v2026.2.2.3396` that causes the game to crash based on game library dependencies.

## Version 0.2.0 2026-01-04 - More of the basics implemented

This release adds more of the basic features that needs implementation. Most of all, contracts can be loaded from other mods now, allowing others to create contracts. In the next version(s) expect more features related to missions and contract packs.

At the core, the prerequisites for offering a contract have been expanded, as well as the triggers for actions.  
All the attributes for the orbit requirement have been implemented.

Stay tuned for more updates soon.

Check the [changelog](CHANGELOG.md) for all the details.

Supported KSA versions:
- `v2025.12.31.3103` fully supported
- `v2025.12.33.3123` play supported, loading save-game is broken due to KSA issue.

## Version 0.1.0 2025-12-30 - First version

This is the first released (very alpha) version of this mod. It has the basic implementations to close the loop to offer, accept and complete a contract. 
It still needs a lot of work to offer something close to a set of contracts that can be consiered as tutorials or a campaign.

For now, please try out this mod and give me feedback;
- Which features would you like to see?
- Did you encounter a bug?
- As a fellow modder/coder, how can the mod and it's backend be improved?

Supported KSA versions:
- `v2025.12.31.3103` fully supported
- `v2025.12.33.3123` play supported, loading save-game is broken due to KSA issue.
