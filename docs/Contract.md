# Contracts

Contracts in Contract Manager are instanciated from a contract blueprint `ContractBlueprint` class. These blueprints are defined in XML and read from the `contracts` folder in mod folders in `Content` folder.

## `ContractBlueprint` Class

The base xml tag for the contract blueprint is `<Contract>`. The following table describes the mapping between the `ContractBlueprint` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `uid`          | `<uid>`      | Unique identifier of the contract blueprint | 
| `title`        | `<title>`    | Title of the contract                       |
| `synopsis`     | `<synopsis>` | Short summary of the contract               |
| `description`  | `<description>` | Description of the contract              |
| `prerequisites` | `<prerequisites>` | Prerequisites for the contract, contains [`Prerequisite`](#prerequisite-class) |
| `requirements` | `<requirements>` | Requirements for the contract, contains [`Requirement`](#requirement-class)       |
| `completionCondition` | `<completionCondition>` | Completion condition of the contract based on the requirements. One of [`CompletionConditions`](#completionconditions) |
| `actions`      | `<actions>`  | Actions to be executed as part of the contract, contains [`Action`](#action-class) |

### `Prerequisite` Class

The prerequisites for offering a contract are defined with the `Prerequisite` class.

The `<prerequisites>` tag contains one or more `<Prerequisite>` tags. Each `<Prerequisite>` tag maps to a `Prerequisite` class instance.
The following table describes the mapping between the `Prerequisite` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `type`         | `<type>`   | Type of the prerequisite. One of [`PrerequisiteType`](#prerequisitetype) |
| | |  Below fields as needed by `PrerequisiteType` |
| `maxNumOfferedContracts` | `<maxNumOfferedContracts>` | Offer contract if number of offered contracts is less than this number. Used if `prerequisiteType` is `maxNumOfferedContracts` |
| `maxNumAcceptedContracts` | `<maxNumAcceptedContracts>` | Offer contract if number of accepted contracts is less than this number.  Used if `prerequisiteType` is `maxNumAcceptedContracts` |

#### `PrerequisiteType`

The following types are supported:
* `maxNumOfferedContracts`: Offer contract if number of offered contracts is less than number defined in `maxNumOfferedContracts` field.
* `maxNumAcceptedContracts`:  Offer contract if number of accepted contracts is less than number defined in `maxNumAcceptedContracts` field.

In the future more types will be added, such as:

* `maxCompleteCount`: contract has been completed less than this int number of times.
* `maxConcurrentCount`: contract has less than this int number of accepted instances of this contract.
* `maxOfferCount`: contract has less than this int number of offered instances of this contract.
* `unlockedTech`: Unlocked technology / node in science tree.
* `minMoney`: player has more than this amount of money.
* `maxMoney`: player has less than this amount of money.
* `minFame`: player has more than this amount of fame.
* `maxMoney`: player has less than this amount of fame.
* `minNumberOfVessels`: player has more than this amount of active vessels
* `maxNumberOfVessels`: player has less than this amount of active vessels


### `Requirement` Class

The requirements for completing a contract are defined with the `Requirement` class. 

The `<requirements>` tag contains one or more `<Requirement>` tags. Each `<Requirement>` tag maps to a `Requirement` class instance.
The following table describes the mapping between the `Requirement` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `uid`          | `<uid>`    | Unique identifier of the requirement        |
| `type`         | `<type>`   | Type of the requirement. One of [`RequirementType`](#requirementtype) |
| `title`        | `<title>`  | Title of the requirement                    |
| `synopsis`     | `<synopsis>` | Short summary of the requirement            |
| `description`  | `<description>` | Description of the requirement          |
| `isCompletedOnAchievement` | `<isCompletedOnAchievement>` | Flag if the requirement is completed upon achievement. If false, the required condition has to be maintained until the contract or the parent requirement is completed. |
| `isHidden`   | `<isHidden>` | Flag if the requirement is hidden until previous requirement was achieved. |
| `completeInOrder` | `<completeInOrder>` | Flag if the requirement can only be completed after previous requirement was achieved. |
| `requirements` | `<requirements>` | Child requirements for this requirement, contains `Requirement` |
| `completionCondition` | `<completionCondition>` | Completion condition of the requirement based on the child requirements. One of [`CompletionConditions`](#completionconditions) |
| | |  Below fields as needed by `RequirementType` |
| `orbit`       | `<Orbit>`  | Orbit parameters for `orbit` type requirement, contains [`RequiredOrbit`](#requiredorbit-class) |

#### `RequirementType`

The following types are supported:

* `orbit`: Require specific orbit as defined by [`RequiredOrbit`](#requiredorbit-class) to be achieved.

More requirement types will be added in the future, such as:

* `vesselState`: state of the vessel.
* `hasResources`: have resources
* `hasCrew`: has specified crew
* `hasPassengers`: has specified passengers
* `hasPart`: has part
* `hasModule`: has a module (e.g. SAS, gyro etc.)

#### `CompletionConditions`

The completion conditions of the contract or requirement. The following conditions are supported:
* `all`: All requirements must be completed.
* `any`: Any requirement must be completed.

In the future more conditions may be added, like `atLeast`.

#### `RequiredOrbit` Class

A requirement of type `orbit` needs specific orbit parameters. These are defined with the `RequiredOrbit` class.  
The following table describes the mapping between the `RequiredOrbit` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `targetBody` | `<targetBody>` | Celestial body around which the orbit must be achieved. |
| `minApoapsis` | `<minApoapsis>` | Minimum apoapsis altitude in meters.    |
| `maxApoapsis` | `<maxApoapsis>` | Maximum apoapsis altitude in meters.    |
| `minPeriapsis` | `<minPeriapsis>` | Minimum periapsis altitude in meters.  |
| `maxPeriapsis` | `<maxPeriapsis>` | Maximum periapsis altitude in meters.  |

In the future more orbit parameters will be added:

* `status`: one of `orbiting`, `suborbit`, `escape` (subject to change)
* `minInclination`: minimum inclination
* `maxInclination`: maximum inclination
* `minEccentricity`: minimum eccentricity (related to Apoapsis and Periapsis)
* `maxEccentricity`: maximum eccentricity (related to Apoapsis and Periapsis)
* `minOrbitTime`: minimum orbit time
* `maxOrbitTime`: maximum orbit time
* `minLongitudeOfAscendingNode`: minimum angle between body reference and ascending node
* `maxLongitudeOfAscendingNode`: maximum angle between body reference and ascending node
* `minArgumentOfPeriapsis`: minimum angle between ascending node and Periapsis
* `maxArgumentOfPeriapsis`: maximum angle between ascending node and Periapsis
* `minSemiMajorAxis`: minimum semi major axis (related to Apoapsis and Periapsis)
* `maxSemiMajorAxis`: maximum semi major axis (related to Apoapsis and Periapsis)

### `Action` Class

The actions to be executed as part of the contract are defined with the `Action` class. Each action is triggered and a certain action is exectuted.

The <actions> tag contains one or more <Action> tags. Each <Action> tag maps to a `Action` class instance. 
The following table describes the mapping between the `Action` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `trigger`      | `<trigger>` | Trigger for the action. One of [`TriggerType`](#triggertype) |
| `type`         | `<type>`   | Type of the action to execute. One of [`ActionType`](#actiontype) |
| | |  Below fields as needed by `ActionType` |
| `showMessage` | `<showMessage>` | Message to show to the player. Used when `ActionType` is `showMessage`. |

#### `TriggerType`

The triggers for the actions. The following triggers are supported:

* `onContractComplete`: when the contract is completed.
* `onContractFail`: when the contract is failed. (for now also when canceled)

In the futrure more triggers will be added:

* `onContractOffer`: when the contract is offered.
* `onContractAccepted`: when the contract is accepted.
* `onContractDecline`: when the contract is declined.
* `onContractCancel`: when the contract is canceled.
* `onRequirementActivated`: When the requirement activated (i.e. became active as the next one to execute).
* `onRequirementComplete`: When the requirement completed.
* `onRequirementFail`: When the requirement failed.

#### `ActionType`

The type of action to be executed. The following action types are supported:

* `showMessage`: Show a message to the player.

In the futrure more triggers will be added in the future, such as:
* `spawnVessel`: Spawn a vessel.
* `giveReward`: Give a reward to the player. Can be used to give reward for completing/failing contract but also requirements.

