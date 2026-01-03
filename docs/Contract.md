# Contracts

Contracts in Contract Manager are instanciated from a contract blueprint `ContractBlueprint` class. These blueprints are defined in XML and read from the `contracts` folder in mod folders in `Content` folder.

## `ContractBlueprint` Class

The base xml tag for the contract blueprint is `<Contract>`. The following table describes the mapping between the `ContractBlueprint` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `version`      | `<version>`  | Version of the Contract Manager mod for which this contract was created. | 
| `uid`          | `<uid>`      | Unique identifier of the contract blueprint | 
| `title`        | `<title>`    | Title of the contract                       |
| `synopsis`     | `<synopsis>` | Short summary of the contract               |
| `description`  | `<description>` | Description of the contract              |
| `expiration`  | `<expiration>` | Time in seconds after which the offered contract expires, default is to never expire. |
| `isRejectable`  | `<isRejectable>` | Flag to allow rejecting a contract, default is true. |
| `deadline`  | `<deadline>` | Time in seconds after which the accepted contract fails, default is no deadline. |
| `isAutoAccepted`  | `<isAutoAccepted>` | Flag to auto-accept an offered contract, default is false. |
| `prerequisites` | `<prerequisites>` | Prerequisites for the contract, contains [`Prerequisite`](#prerequisite-class) |
| `completionCondition` | `<completionCondition>` | Completion condition of the contract based on the requirements. One of [`CompletionConditions`](#completionconditions) |
| `requirements` | `<requirements>` | Requirements for the contract, contains [`Requirement`](#requirement-class)       |
| `actions`      | `<actions>`  | Actions to be executed as part of the contract, contains [`Action`](#action-class) |

#### `CompletionConditions`

The completion conditions of the contract or requirement. The following conditions are supported:
* `all`: All requirements must be completed.
* `any`: Any requirement must be completed.

In the future more conditions may be added, like `atLeast`.

### `Prerequisite` Class

The prerequisites for offering a contract are defined with the `Prerequisite` class.

The `<prerequisites>` tag contains one or more `<Prerequisite>` tags. Each `<Prerequisite>` tag maps to a `Prerequisite` class instance.
The following table describes the mapping between the `Prerequisite` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `type`         | `<type>`   | Type of the prerequisite. One of [`PrerequisiteType`](#prerequisitetype) |
| | |  Below fields as needed by `PrerequisiteType` |
| `maxNumOfferedContracts` | `<maxNumOfferedContracts>` | Offer contract if number of offered contracts is less than this number (Default: unlimited). Used if `prerequisiteType` is `maxNumOfferedContracts` |
| `maxNumAcceptedContracts` | `<maxNumAcceptedContracts>` | Offer contract if number of accepted contracts is less than this number (Default: unlimited).  Used if `prerequisiteType` is `maxNumAcceptedContracts` |
| `maxCompleteCount` | `<maxCompleteCount>` | Offer contract if number of completed instances of this contract blueprint is less than this number (Default: 0). Used if `prerequisiteType` is `maxCompleteCount` |
| `maxFailedCount` | `<maxFailedCount>` | Offer contract if number of failed instances of this contract blueprint is less than this number (Default: unlimited). Used if `prerequisiteType` is `maxFailedCount` |
| `maxConcurrentCount` | `<maxConcurrentCount>` | Offer contract if number of accepted contracts of this contract blueprint is less than this number (Default: 0). Used if `prerequisiteType` is `maxConcurrentCount` |
| `hasCompletedContract` | `<hasCompletedContract>` | Offer contract if a contract with the defined contract blueprint uid has been completed. Used if `prerequisiteType` is `hasCompletedContract` |
| `hasFailedContract` | `<hasFailedContract>` | Offer contract if a contract with the defined contract blueprint uid has been failed. Used if `prerequisiteType` is `hasFailedContract` |
| `hasAcceptedContract` | `<hasAcceptedContract>` | Offer contract if a contract with the defined contract blueprint uid has been accepted (and not yet completed). Used if `prerequisiteType` is `hasAcceptedContract` |
| `minNumberOfVessels` | `<minNumberOfVessels>` | Offer contract if there are more than this number of vessels in the current celestial system. Used if `prerequisiteType` is `minNumberOfVessels` |
| `maxNumberOfVessels` | `<maxNumberOfVessels>` | Offer contract if there are less than this number of vessels in the current celestial system. Used if `prerequisiteType` is `maxNumberOfVessels` |

> The number of vessels is determined only within the current celestial system. It also does not differentiate between actual vessels and kittens in EVA. Also, it does not differentiate if the vessel is owned/controllable by the player.

#### `PrerequisiteType`

The following types are supported:
* `maxNumOfferedContracts`: Offer contract if number of offered contracts is less than number defined in `maxNumOfferedContracts` field.
* `maxNumAcceptedContracts`: Offer contract if number of accepted contracts is less than number defined in `maxNumAcceptedContracts` field.
* `maxCompleteCount`: Offer contract if number of completed instances of this contract blueprint  is less than number defined in `maxCompleteCount` field.
* `maxFailedCount`: Offer contract if number of failed instances of this contract blueprint is less than number defined in `maxFailedCount` field.
* `maxConcurrentCount`: Offer contract if number of accepted contracts of this contract blueprint is less than number defined in `maxConcurrentCount` field.
* `hasCompletedContract`: Offer contract if a contract with contract blueprint uid as defined in `hasCompletedContract` field has been completed.
* `hasFailedContract`: Offer contract if a contract with contract blueprint uid as defined in `hasFailedContract` field has been failed.
* `hasAcceptedContract`: Offer contract if a contract with contract blueprint uid as defined in `hasAcceptedContract` field has been accepted (and not yet completed).
* `minNumberOfVessels`: There are more than this number of vessels in the current celestial system.
* `maxNumberOfVessels`: There are less than this number of vessels in the current celestial system.

In the future more types will be added, such as:

* `unlockedTech`: Unlocked technology / node in science tree.
* `minMoney`: player has more than this amount of money.
* `maxMoney`: player has less than this amount of money.
* `minFame`: player has more than this amount of fame.
* `maxMoney`: player has less than this amount of fame.


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
| | |  Below fields as needed by `RequirementType` |
| `orbit`       | `<Orbit>`  | Orbit parameters for `orbit` type requirement, contains [`RequiredOrbit`](#requiredorbit-class) |
| `group`       | `<Group>`  | Group parameters for `group` type requirement, contains [`RequiredGroup`](#requiredgroup-class) |

#### `RequirementType`

The following types are supported:

* `orbit`: Require specific orbit as defined by [`RequiredOrbit`](#requiredorbit-class) to be achieved.
* `group`: A group of requirements as defined by [`RequiredGroup`](#requiredgroup-class).

More requirement types will be added in the future, such as:

* `vesselState`: state of the vessel.
* `hasResources`: have resources
* `hasCrew`: has specified crew
* `hasPassengers`: has specified passengers
* `hasPart`: has part
* `hasModule`: has a module (e.g. SAS, gyro etc.)

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
| `minEccentricity` | `<minEccentricity>` | Minimum eccentricity, ratio: `0.0`: circular, `< 1.0` elliptical, `1.0` parabolic, `> 1.0` hyperbolic. |
| `maxEccentricity` | `<maxEccentricity>` | Maximum eccentricity, ratio: `0.0`: circular, `< 1.0` elliptical, `1.0` parabolic, `> 1.0` hyperbolic. |
| `minPeriod` | `<minPeriod>` | Minimum period, orbiting time in seconds. |
| `maxPeriod` | `<maxPeriod>` | Maximum period, orbiting time in seconds. |
| `minLongitudeOfAscendingNode` | `<minLongitudeOfAscendingNode>` | Minimum longitude of ascending node, angle (0-360 degrees) from reference frame of the parent body to the ascending node in reference plane. |
| `maxLongitudeOfAscendingNode` | `<maxLongitudeOfAscendingNode>` | Maximum longitude of ascending node, angle (0-360 degrees) from reference frame of the parent body to the ascending node in reference plane. |
| `minInclination` | `<minInclination>` | Minimum inclination, angle (?-? degrees) from the reference plane of the parent body to the orbital plane. |
| `maxInclination` | `<maxInclination>` | Maximum inclination, angle (?-? degrees) from the reference plane of the parent body to the orbital plane. |
| `minArgumentOfPeriapsis` | `<minArgumentOfPeriapsis>` | Minimum argument of periapsis, angle (0-360 degrees) from the ascending node to the periapsis in orbital plane. |
| `maxArgumentOfPeriapsis` | `<maxArgumentOfPeriapsis>` | Maximum argument of periapsis, angle (0-360 degrees) from the ascending node to the periapsis in orbital plane. |
| `orbitType` | `<orbitType>` | The type of orbit, one of [`OrbitType`](#orbittype). |

#### `OrbitType`

The type of orbit, defined as:

* `elliptical`: An elliptical orbit around a body.
* `suborbit`: An orbit with the Periapsis within the radius of the orbited body.
* `escape`: An orbit with the Apoapsis outside the Sphere of Influence (SOI) of the orbited body.

#### `RequiredGroup` Class

A requirement of type `group` holds a list of requirements. Based on the `completionCondition` either any or all of these child requirements need to be completed.
The following table describes the mapping between the `RequiredGroup` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `completionCondition` | `<completionCondition>` | Completion condition of the requirement based on the child requirements. One of [`CompletionConditions`](#completionconditions) |
| `requirements` | `<requirements>` | Child requirements for this requirement, contains `Requirement` |

### `Action` Class

The actions to be executed as part of the contract are defined with the `Action` class. Each action is triggered and a certain action is exectuted.

The `<actions>` tag contains one or more `<Action>` tags. Each `<Action>` tag maps to a `Action` class instance. 
The following table describes the mapping between the `Action` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `trigger`      | `<trigger>` | Trigger for the action. One of [`TriggerType`](#triggertype) |
| `type`         | `<type>`   | Type of the action to execute. One of [`ActionType`](#actiontype) |
| | |  Below fields as needed by `ActionType` |
| `showMessage` | `<showMessage>` | Message to show to the player. Used when `ActionType` is `showMessage` or `showBlockingPopup`. |

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
* `showBlockingPopup`: Show a blocking popup message to the player.

In the futrure more triggers will be added in the future, such as:
* `spawnVessel`: Spawn a vessel.
* `giveReward`: Give a reward to the player. Can be used to give reward for completing/failing contract but also requirements.

