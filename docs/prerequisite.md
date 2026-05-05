# `Prerequisite` Class

The prerequisites for offering a contract or mission are defined with the `Prerequisite` class.

The `<Prerequisite>` tag maps to a `Prerequisite` class instance.
The following table describes the mapping between the `Prerequisite` class variables and their corresponding XML tags.

| class variable | xml tag    | description                                 |
| :------------- | :--------- | :------------------------------------------ |
| `maxNumOfferedContracts` | `<maxNumOfferedContracts>` | Offer contract if number of offered contracts is less than this number (Default: unlimited). |
| `maxNumAcceptedContracts` | `<maxNumAcceptedContracts>` | Offer contract if number of accepted contracts is less than this number (Default: unlimited).  |
| `maxNumOfferedMissions` | `<maxNumOfferedMissions>` | Offer mission if number of offered missions is less than this number (Default: unlimited). |
| `maxNumAcceptedMissions` | `<maxNumAcceptedMissions>` | Offer mission if number of accepted missions is less than this number (Default: unlimited).  |
| `maxCompleteCount` | `<maxCompleteCount>` | Offer if number of completed instances of this contract blueprint is less than this number (Default: 0). |
| `maxFailedCount` | `<maxFailedCount>` | Offer if number of failed instances of this contract blueprint is less than this number (Default: unlimited). |
| `maxConcurrentCount` | `<maxConcurrentCount>` | Offer if number of accepted contracts of this contract blueprint is less than this number (Default: 0). |
| `hasCompletedContract` | `<hasCompletedContract>` | Offer if a contract with the defined contract blueprint uid has been completed. |
| `hasFailedContract` | `<hasFailedContract>` | Offer if a contract with the defined contract blueprint uid has been failed. |
| `hasAcceptedContract` | `<hasAcceptedContract>` | Offer if a contract with the defined contract blueprint uid has been accepted (and not yet completed). |
| `hasCompletedMission` | `<hasCompletedMission>` | Offer if a contract with the defined contract blueprint uid has been completed. |
| `hasFailedMission` | `<hasFailedMission>` | Offer if a contract with the defined contract blueprint uid has been failed. |
| `hasAcceptedMission` | `<hasAcceptedMission>` | Offer if a contract with the defined contract blueprint uid has been accepted (and not yet completed). |
| `minNumberOfVessels` | `<minNumberOfVessels>` | Offer if there are more than this number of vessels in the current celestial system. |
| `maxNumberOfVessels` | `<maxNumberOfVessels>` | Offer if there are less than this number of vessels in the current celestial system. |

> The number of vessels is determined only within the current celestial system. It also does not differentiate between actual vessels and kittens in EVA. Also, it does not differentiate if the vessel is owned/controllable by the player.

In the future more fields will be added, such as:

* `unlockedTech`: Unlocked technology / node in science tree.
* `minMoney`: player has more than this amount of money.
* `maxMoney`: player has less than this amount of money.
* `minFame`: player has more than this amount of fame.
* `maxMoney`: player has less than this amount of fame.
