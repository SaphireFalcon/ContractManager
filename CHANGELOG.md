# Changelog

## Unreleased

### Added

#### GUI

#### Core
- Add `MissionBlueprint` data structure as a blue print to create missions.
- Add `Mission` data structure to hold a contract instanciated from a `ContractBlueprint`.
- Add `MissionBlueprintUID` field to `ContractBlueprint` to link it to a `MissionBlueprint`.
- Add `missionUID` field to `Contract` data structure to link an offered contract to an accepted mission.
- Add offering missions
- Add functionality to set `Contract.missionUID` with `missionUID` of the linked mission.
- Add functionality to add `ContractUID` to `Mission.contractUIDs` to link mission to contract(s).
- Add function `ContractUtils.FindContractFromContractUID`
- Add `MissionUtils` with functions `FindContractFromContractUID`
- Add function `DoAction(Mission)` to do actions triggered by a mission.
- Add function `ShowMessage(Mission)` to show a message triggered by a mission.
- Add generate function for a mission with 2 contracts to fly to Luna and back to Earth.
- Add example mission with 2 contracts to fly to Luna and back to Earth.
- Add `maxNumberOfOfferedMissions` and `maxNumberOfAcceptedMissions` fields to `ContractManagerData`.

### Changed
- Reordered `ContractStatus` from `Failed` -> `Completed`.
- Renamed `ContractUtils.FindContractFromUID` to `ContractUtils.FindContractFromBlueprintUID`, and changed return type from `Contract?` to `List<Contract>`, because it could be possible to have multiple contracts instanciated from the same blueprint.
- Reordered the `PrerequisiteType` to group by specific and generic types.
- Changed to only offer a contract if the linked mission is accepted.

### Fixed

- Fix to check all required orbit fields if the required orbit is achieved.
- Fix to show in *Active Contracts Window* the required orbited body and orbit type.
- Fix to show max Apoapsis/Periapsis if the requirement was not yet started.

## [0.2.0] - 2026-01-04

### Added

#### GUI
- Show expiration of an offered contract in contract details of *Contract Management Window*. (#71)
- Gray-out the accept button if the offered contract cannot be accepted because of `maxNumberOfAcceptedContracts`. (#71)
- Gray-out the reject button if the offered contract cannot be rejected because of `isRejectable`. (#71)
- Show deadline of an accepted contract in contract details of *Contract Management Window*. (#72)
- Show deadline of an accepted contract in *Active Contracts Window*. (#72)
- Show the added orbit requirements in *Contract Management Window* and *Active Contracts Window*. (#76)

#### Core
- Add typing for the supported primitives for XML serialization. (#66)
- Add validation methods to contract blueprint and fields. (#68)
- Add try-catch to catch XML serialization errors. (#68)
- Add loading contract blueprints from other (mod) folders. (#69)
- Add 'expiration' as a field to contract blueprint. (#71)
- Add expiring an offered contract. (#71)
- Add 'isRejectable' as a field to contract blueprint. (#71)
- Add 'deadline' as a field to contract blueprint. (#72)
- Add failing of an accepted contract after the deadline passed. (#72)
- Add 'isAutoAccepted' as field to contract blueprint. (#72)
- Add when offering a contract to auto-accept when `isAutoAccepted` is true. (#72)
- Add 'maxCompleteCount', 'maxFailedCount', 'maxConcurrentCount' as `PrerequisiteType`. (#74)
- Add checks to offer contract only if the contract has not been completed/failed/accepted a number of times. (#74)
- Add 'hasCompletedContract', 'hasFailedContract', 'hasAcceptedContract' as `PrerequisiteType`. (#74)
- Add checks to offer contract only if a specified contract blueprint has been completed/failed/accepted. (#74)
- Add 'minNumberOfVessels', 'maxNumberOfVessels' as `PrerequisiteType`. (#74)
- Add checks to offer contract only if a specified number of vehicles are present in the current celestial system. (#74)
- Add the remaining orbit parameters to `RequiredOrbit` and `TrackedOrbit` types. (#76)
- Add additional trigger types to `Action`. (#81)
- Add triggering the newly added `TriggerType`. (#81)

#### Doc
- Updated the documentation for the added fields in `ContractBlueprint`. (#74)
- Updated the documentation for the added `PrerequisiteTypes`. (#74)
- Updated the documentation for the added fields in `RequiredOrbit`. (#76)
- Updated the documentation for the added`TriggerTypes`. (#81)

### Changed

- Moved `ActionType` outside the `Action` class definition. (#67)
- Moved `TriggerType` outside the `Action` class definition. (#67)
- Reverted the `PopupWindow` contructor to create a popup based on `title`, `uid`, `messageToShow`. (#81)

### Fixed
- Fix using `Universe.GetElapsedSimTime()` instead of `Program.GetPlayerTime()` to get the in-game time. (#71)
- Fix use Colors constants for the accept and reject button. (#71)
- Fix to not offer the same contract multiple times. (#74)
- Fix ImGui issue when showing multiple contracts with the same title in the same contracts tab. (#79)
- Fix ImGui issue when showing popup multiple times for contracts with the same title. (#79)
- Fix issue offering same contract multiple times. (#79)
- Fix setting all maintained child requirements to achieved when the parent requirement is set to achieved. (#81)

## [0.1.0] - 2025-12-30

### Added

#### GUI
- Add **Contract Management Window** to allow users to view offered, accepted, and finished contracts.
- Add viewing details of a selected contract to *Contract Management Window*.
- Add accepting and rejecting contracts to *Contract Management Window*.
- Add **Active Contracts Window** to allow users to view accepted contract and their requirement in a collapsible tree list.
- Add showing the status of requirements in the *Active Contracts Window* with colors.
- Add showing the tracked state of requirement values in the *Contract Management Window*.
- Add a button to *Active Contracts Window* to show details of a contract in the *Contract Management Window*.
- Add functionality to show a small window for the `ShowMessage` action
- Add functionality to show a blocking popup (modal) for the `ShowBlockingPopup` action, and pause the game.

#### Core
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

#### Doc
- Add documentation of the Contract Blueprint data structure [Contract](https://github.com/SaphireFalcon/ContractManager/blob/master/docs/Contract.md).
