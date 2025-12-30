# Changelog

## [0.1.0] - 2025-12-30

### Added

GUI
- Add **Contract Management Window** to allow users to view offered, accepted, and finished contracts.
- Add viewing details of a selected contract to *Contract Management Window*.
- Add accepting and rejecting contracts to *Contract Management Window*.
- Add **Active Contracts Window** to allow users to view accepted contract and their requirement in a collapsible tree list.
- Add showing the status of requirements in the *Active Contracts Window* with colors.
- Add showing the tracked state of requirement values in the *Contract Management Window*.
- Add a button to *Active Contracts Window* to show details of a contract in the *Contract Management Window*.
- Add functionality to show a small window for the `ShowMessage` action
- Add functionality to show a blocking popup (modal) for the `ShowBlockingPopup` action, and pause the game.

Core
- Add `ContractBlueprint` data structure readable (and writable) from disk used as a blue print to create offered contracts.
- Add `Prerequisite` data structure used by `ContractBlueprint` to determine when a contract can be offered templated by this blueprint.
- Add `Requirement` data structure used by `ContractBlueprint` to configure what conditions need to be achieved to complete the contract.
- Add `Action` data structure used by `ContractBlueprint` to configure which action should be performed on during certain events.
- Add `PrerequisiteType` enum to `Prerequisite` adding `MaxNumOfferedContracts` and `MaxNumAcceptedContracts` prerequisite.
- Add `RequirementType` enum to `Requirement` adding `Orbit` and `Group` as types of requirements.
- Add `RequiredOrbit` data structure used by `Requirement` when `RequirementType` is `Orbit` to configure what orbit needs to be achieved to achieve the requirement.
- Add `RequiredGroup` data structure used by `Requirement` when `RequirementType` is `Group` holding multiple child `Requirement`.
- Add `ActionType` enum to `Action` adding `ShowMessage` and `ShowBlockingPopup` to configure the type of action to execute.
- Add `TriggerType` enum to `Action` adding `OnContractComplete` and `OnContractFail` to configure when the action should be executed.
- Add `Contract` data structure to hold a contract instanciated from a `ContractBlueprint`.
- Add `TrackedRequirement` data structure used by `Contract`.
- Add `TrackedOrbit` data structure inheriting from `TrackedRequirement` to track the status of a `RequiredOrbit`.
- Add `TrackedGroup` data strucuture inheriting from `TrackedRequirement` to track the status of a `RequiredGroup`.
- Add functionality to `ContractManager` to offer a contract based on a `ContractBlueprint` when the prerequisites are met.
- Add functionality to `Contract` and `TrackedRequirement` (and childs) to update the state that is tracked.
- Add functionality to `Contract` and `TrackedRequirement` (and childs) to update the status of a requiremet.
- Add functionality to `Contract` to check if all requirements are achieved and the contract is completed.
- Add functionality to `Contract` to check if any requirement is failed and the contract is failed.
- Add functionality to `Contract` to trigger actions.
- Add functionality to load blueprint contracts from the `contracts` folder in the `ContractManager` mod folder.
- Add `ContractManagerData` data structure holding the internal data used by `ContractManager` mod.
- Add functionality to save and load the `ContractManagerData` data as part of the in-game load/save method.

Doc
- Add documentation of the Contract Blueprint data structure [Contract](https://github.com/SaphireFalcon/ContractManager/blob/master/docs/Contract.md).
