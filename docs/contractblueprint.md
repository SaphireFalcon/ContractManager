# `ContractBlueprint` Class

The base xml tag for the contract blueprint is `<Contract>`. The following table describes the mapping between the `ContractBlueprint` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `version`      | `<version>`  | Version of the Contract Manager mod for which this contract was created. |
| `uid`          | `<uid>`      | Unique identifier of the contract blueprint (max 128 char) | 
| `missionBlueprintUID`		| `<missionBlueprintUID>`      | Unique identifier of the mission blueprint associated with this contract (max 128 char) | 
| `title`        | `<title>`    | Title of the contract (max 128 char)                       |
| `synopsis`     | `<synopsis>` | Short summary of the contract (max 1024 char)               |
| `description`  | `<description>` | Description of the contract (max 4096 char)              |
| `expiration`  | `<expiration>` | Time in seconds after which the offered contract expires, default is to never expire. |
| `isRejectable`  | `<isRejectable>` | Flag to allow rejecting a contract, default is true. |
| `deadline`  | `<deadline>` | Time in seconds after which the accepted contract fails, default is no deadline. |
| `isAutoAccepted`  | `<isAutoAccepted>` | Flag to auto-accept an offered contract, default is false. |
| `prerequisite` | `<prerequisite>` | Prerequisite for the contract, of type [`Prerequisite`](#prerequisite-class) |
| `completionCondition` | `<completionCondition>` | Completion condition of the contract based on the requirements. One of [`CompletionConditions`](#completionconditions) |
| `requirements` | `<requirements>` | Requirements for the contract, contains [`Requirement`](./requirement.md)       |
| `actions`      | `<actions>`  | Actions to be executed as part of the contract, contains [`Action`](./action.md) |

## `CompletionConditions`

The completion conditions of the contract or requirement. The following conditions are supported:
* `all`: All requirements must be completed.
* `any`: Any requirement must be completed.

In the future more conditions may be added, like `atLeast`.
