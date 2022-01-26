# ACC (AccuracyComboCounter)
## Main Features

- Show a counter of your acc combo which is the amount of consecutive hits with a score above (and equal to) a user-defined accuracy threshold.
- Add an optional counter showing the highest combo you got so far
- Add an optional counter showing the amount of cuts below the set accuracy threshold. This counter is similar to the "Misses" counter of Counters+.
- Show on the results screen the amount of good cuts compared to the total cuts as well as your highest combo. (disableable)
- Select which events break your combo. (Missing blocks, bad cuts, hitting bombs, hitting walls).
- Highly customisable counter text.

## Known Limitations

- Changing the scale of the canvas or rotating the canvas doesn't work as intended. Using a custom canvas distance may also break the formatting of the counter.
- The counter is disabled on circle environments.

## Settings
### Main Settings
| Setting name | Explanation / Info | Default value |
| --- | --- | --- |
| Accuracy Threshold | A hit score below this score will break the Accuracy Combo. | `0` |
| Show On Results Screen | Having this enabled will make the number of good cuts and your max combo show up on the results screen. | `true` |
| Hide Combo Break Animation | Enable this if you do not want the combo break animation to show. | `false` |

### Extra Counters Settings
| Setting name | Explanation / Info | Default value |
| --- | --- | --- |
| Max Combo Position<br><br><br>Low Acc Cuts Position | Select where the extra counter should be displayed relative to the Accuracy Combo Counter. Selecting a position below the counter will move the Accuracy Combo Counter up. <br>The options are:<br>- `Above +2`<br>- `Above +1`<br>- `Disabled`<br>- `Below -1`<br>- `Below -2` | `Disabled`<br><br><br>`Disabled` |

### Combo Breaking Events Settings
| Setting name | Explanation / Info | Default value |
| --- | --- | --- |
| Break On Miss | Disabling this will ignore missed blocks. This also means that it won't be added to the total note count. | `true` |
| Break On Bad Cut | Disabling this will ignore any bad cuts. This also means that it won't be added to the total note count. | `true` |
| Break On Bomb | Disabling this will ignore any cut bombs. | `true` |
| Break On Wall | Disabling this will stop the combo from breaking when you hit your head against a wall. | `true` |

### Counter Text Settings
| Setting name | Explanation / Info | Default value |
| --- | --- | --- |
| Combo Label Text | Customise the text of the label from the Accuracy Combo Counter. | `"Combo > %t"` |
| Combo Counter Text | Customise the score text from the Accuracy Combo Counter. | `"%c"` |
| Max Combo Counter Text | Customise the text used for the Max Combo Counter. | `"Max Combo : %m"` |
| Low Acc Cuts Counter Text | Customise the text used for the Low Acc Cuts Counter. | `"Cuts Below %t : %l"` |

### Results View Text Settings (Only available in `UserData/AccuracyComboCounter.json`)
| Setting name | Explanation / Info | Default value |
| --- | --- | --- |
| Result Text | Customise the text that shows your good cuts and the note count on the results screen. | `"%h<size=70%> / %n</size>"` |
| Max Combo Text | Change the text that appears on the results screen under the Result Text when your combo has been broken. | `"MAX ACC COMBO %m"` |
| Full Combo Text | Change the text that appears on the results screen under the Result Text when you get a Full Combo. | `"FULL ACC COMBO"` |


## String Formatting
You can change any string into anything you like. For example, you can use size tags `<size=90%>text</size>` or color tags `<color=#b00b69>text</color>`.
The values are inserted into the string with identifiers. These are a `%` followed by a letter. Both upper and lower case letters work. Any identifier can be used in any string, so could even put everything into one of the extra counters if you so desire.

### Identifiers
| Value | Identifier | Explanation / Info |
| --- | --- | --- |
| Acc Threshold | %t | The value set with the Accuracy Threshold setting. A cut score below this value will break your Accuracy Combo. |
| Acc Combo | %c | The number of consecutive cuts above or equal to the Accuracy Threshold. |
| Max Acc Combo | %m | The highest amount of good cuts you had before breaking your combo. |
| Low Acc Cuts | %l | The number of cuts below the Accuracy Threshold value. |
| High Acc Cuts | %h | The number of cuts above or equal to the Accuracy Threshold value. |
| NoteCount (Total Cuts) | %n | The total number of blocks. Misses will not be counted if `Break On Miss` is disabled and bad cuts will not be counted if `Break On Bad Cut` is disabled. |


### Want to contribute?
If you want to improve on the mod in any way, please contact me or make a pull request.
