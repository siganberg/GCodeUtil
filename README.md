# GCodeModifier

A lightweight tool that modifies G-code files by inserting custom commands before or after each ToolChange command like `M6` or `M98`. Useful for smarter way to retract/expand AutoDustBoot.

---

## 🚀 Features

- Automatically inserts custom g-code command before and after the toolchange
- Supports multiple insert commands, comma-separated (e.g. `M8,G4 P1`)

---

## 🧰 Usage

```bash
GCodeModifier <input_file_path>
```

| Argument            | Description                                                      |
| ------------------- | ---------------------------------------------------------------- |
| `<input_file_path>` | Path to the `.gcode` file                                        |


--- 

## 🛠️ Configuration

The application supports customization through a settings.json file. You can configure the following properties:

| Property                        | Description                                                                                                                  | Type                     | Default      | Example       |
| ------------------------------- |------------------------------------------------------------------------------------------------------------------------------| ------------------------ | ------------ | ------------- |
| `GCodeToInsertBeforeToolChange` | G-code to insert **before** tool change. This will insert right before the tool change command.                              | String (comma-separated) | `""` (empty) | `"M9,G0 Z10"` |
| `GCodeToInsertAfterToolChange`  | G-code to insert **after** tool change. This will insert after the toolchange and before the first X and Y movement command. | String (comma-separated) | `"M8,G4 P1"` | `"M8,G4 P1"`  |



## 🧪 Examples

Sample NC (demo.nc) file content. 

```
...
N210 (Toolpath:- V-Carve 1 [Clear 1])
N220 ()
N230 (Change to tool End Mill {1/4"} UP CUT)
N240 M98 P631
N250 S19000 M03
N260 M08
N270 G4 P5000
N280 G00 X8.449 Y-0.896
N290 G00
N300 G00 X8.449 Y-0.896
N310 G00 Z3.810
N320 G00 X8.449 Y-0.896
N330 G00 Z2.540
...
```

Executing this command:
```
GCodeModifier demo.nc 
```

Will generate a new NC  (demo_modified.nc) file with the following changes:

```
...
N210 (Toolpath:- V-Carve 1 [Clear 1])
N220 ()
N230 (Change to tool End Mill {1/4"} UP CUT)
N240 M98 P631
N250 S19000 M03
N280 G00 X8.449 Y-0.896
N270 G4 P5000
N280 M8
N281 G4 P1
N290 G00
N300 G00 X8.449 Y-0.896
N310 G00 Z3.810
N320 G00 X8.449 Y-0.896
N330 G00 Z2.540
...
```




This will turn on the flood only after the tool change, then wait 1 second before the spindle plunges into the material. I use this to give the AutoDustBoot time to fully expand.
```
GCodeModifier myfile.gcode "M8,G4 P1" 
```

This will turn on the flood only after the tool change, then wait 5 seconds before the spindle plunges into the material. It’s useful for giving the spindle time to reach full speed before continuing the job. This is especially helpful for PWM spindles that rely on a static delay.
```
GCodeModifier myfile.gcode "M8,G4 P5" 
```

## 📒 Notes

- For latest MacOS (BigSur Up), in order to run this utility you need to codesign it else you will get error `zsh: killed`

```
mkdir <path-where-you-extracted-the-GCodeUtil>
codesign --force --deep --sign - GCodeUtil
```



## ✍️ Author

Developed by FrancisCreation

Pull requests and contributions welcome!




