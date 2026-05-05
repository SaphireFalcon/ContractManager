# `Requirement` Class

The requirements for completing a contract are defined with the `Requirement` class. 

The `<requirements>` tag contains one or more `<Requirement>` tags. Each `<Requirement>` tag maps to a `Requirement` class instance.
The following table describes the mapping between the `Requirement` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `uid`          | `<uid>`    | Unique identifier of the requirement        |
| `type`         | `<type>`   | Type of the requirement. One of [`RequirementType`](#requirementtype) |
| `title`        | `<title>`  | Title of the requirement (max 128 char)       |
| `synopsis`     | `<synopsis>` | Short summary of the requirement (max 1024 char) |
| `description`  | `<description>` | Description of the requirement (max 4096 char) |
| `isCompletedOnAchievement` | `<isCompletedOnAchievement>` | Flag if the requirement is completed upon achievement. If false, the required condition has to be maintained until the contract or the parent requirement is completed. |
| `isHidden`   | `<isHidden>` | Flag if the requirement is hidden until previous requirement was achieved. |
| `completeInOrder` | `<completeInOrder>` | Flag if the requirement can only be completed after previous requirement was achieved. |
| | |  Below fields as needed by `RequirementType` |
| `orbit`       | `<Orbit>`  | Orbit parameters for `orbit` type requirement, contains [`RequiredOrbit`](#requiredorbit-class) |
| `group`       | `<Group>`  | Group parameters for `group` type requirement, contains [`RequiredGroup`](#requiredgroup-class) |

### `RequirementType`

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

## `RequiredOrbit` Class

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

### `OrbitType`

The type of orbit, defined as:

* `elliptical`: An elliptical orbit around a body.
* `suborbit`: An orbit with the Periapsis within the radius of the orbited body.
* `escape`: An orbit with the Apoapsis outside the Sphere of Influence (SOI) of the orbited body.

## `RequiredGroup` Class

A requirement of type `group` holds a list of requirements. Based on the `completionCondition` either any or all of these child requirements need to be completed.
The following table describes the mapping between the `RequiredGroup` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `completionCondition` | `<completionCondition>` | Completion condition of the requirement based on the child requirements. One of [`CompletionConditions`](./contractblueprint.md#completionconditions) |
| `requirements` | `<requirements>` | Child requirements for this requirement, contains `Requirement` |
