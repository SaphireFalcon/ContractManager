
# `MissionBlueprint` Class

The base xml tag for the mission blueprint is `<Mission>`. The following table describes the mapping between the `MissionBlueprint` class variables and their corresponding XML tags.
A contract will reference a mission blueprint via the `missionBlueprintUID` field. A mission blueprint can be referenced by multiple contracts.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `version`      | `<version>`  | Version of the Contract Manager mod for which this mission was created. | 
| `uid`          | `<uid>`      | Unique identifier of the mission blueprint (max 128 char) | 
| `title`        | `<title>`    | Title of the mission (max 128 char)                       |
| `synopsis`     | `<synopsis>` | Short summary of the mission (max 1024 char)               |
| `description`  | `<description>` | Description of the mission (max 4096 char)              |
| `expiration`  | `<expiration>` | Time in seconds after which the offered mission expires, default is to never expire. |
| `isRejectable`  | `<isRejectable>` | Flag to allow rejecting a mission, default is true. |
| `deadline`  | `<deadline>` | Time in seconds after which the accepted mission fails, default is no deadline. |
| `isAutoAccepted`  | `<isAutoAccepted>` | Flag to auto-accept an offered mission, default is false. |
| `prerequisite` | `<prerequisite>` | Prerequisite for the mission, of type [`Prerequisite`](./prerequisite.md) |
| `actions`      | `<actions>`  | Actions to be executed as part of the mission, contains [`Action`](./action.md) |
