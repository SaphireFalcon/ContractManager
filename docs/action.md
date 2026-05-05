# `Action` Class

The actions to be executed as part of the contract are defined with the `Action` class. Each action is triggered and a certain action is exectuted.

The `<actions>` tag contains one or more `<Action>` tags. Each `<Action>` tag maps to a `Action` class instance. 
The following table describes the mapping between the `Action` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `uid`          | `<uid>`    | Unique identifier of the `Action`        |
| `trigger`      | `<trigger>` | Trigger for the action. One of [`TriggerType`](#triggertype) |
| `type`         | `<type>`   | Type of the action to execute. One of [`ActionType`](#actiontype) |
| | |  Below fields as needed by `ActionType` or `TriggerType` |
| `showMessage` | `<showMessage>` | Message to show to the player. Used when `ActionType` is `showMessage` or `showBlockingPopup`. |
| `onRequirement` | `<onRequirement>` | The requirement `uid` that is used to trigger this action. Used when `TriggerType` is any of `OnRequirement*`.. |

## `TriggerType`

The triggers for the actions. The following triggers are supported:

* `onContractOffer`: when the contract is offered.
* `onContractAccept`: when the offered contract is accepted.
* `onContractExpire`: when the offered contract expired.
* `onContractReject`: when the offered or accepted contract is rejected.
* `onContractComplete`: when the accepted contract is completed.
* `onContractFail`: when the accepted contract is failed or passed the deadline.
* `onRequirementStarted`: When the requirement activated (i.e. became active as the next one to execute).
* `onRequirementMaintained`: When the requirement was achieved but needs to be maintained until other requirements are achieved.
* `onRequirementReverted`: When the requirement was being maintained and went back to be tracked.
* `onRequirementAchieved`: When the requirement was achieved.
* `onRequirementFailed`: When the requirement failed.
* `onMissionOffer`: when the mission is offered.
* `onMissionAccept`: when the offered mission is accepted.
* `onMissionExpire`: when the offered mission expired.
* `onMissionReject`: when the offered or accepted mission is rejected.
* `onMissionComplete`: when the accepted mission is completed.
* `onMissionFail`: when the accepted mission is failed or passed the deadline.

In the futrure more triggers can be added, please make a request.

## `ActionType`

The type of action to be executed. The following action types are supported:

* `showMessage`: Show a message to the player.
* `showBlockingPopup`: Show a blocking popup message to the player.

In the futrure more action types will be added in the future, such as:
* `spawnVessel`: Spawn a vessel.
* `giveReward`: Give a reward to the player. Can be used to give reward for completing/failing contract but also requirements.
* `offerContract`: Offer (and auto-accept?) contract.
