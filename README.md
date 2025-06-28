# GCodeModifier

A lightweight tool that modifies G-code files by inserting custom commands after each `M6` or `M98`, just before the first `G0 X.. Y..` movement line that follows. I use this for smarter way to retract/expand AutoDustBoot.

---

## 🚀 Features

- Line-by-line processing for **low memory usage**
- Automatically inserts a configurable G-code command (default: `M8`)
- Supports multiple insert commands, comma-separated (e.g. `M8,G4 P1`)

---

## 🧰 Usage

```bash
GCodeModifier <input_file_path> [insert_gcode]
```

| Argument            | Description                                                      |
| ------------------- | ---------------------------------------------------------------- |
| `<input_file_path>` | Path to the `.gcode` file                                        |
| `[insert_gcode]`    | *(Optional)* G-code(s) to insert, comma-separated. Default: `M8` |

--- 

## 🧪 Examples

This will turn on the flood only after the tool change and before the spindle into the material.
```
# Inserts "M8" before first G0 X.. Y.. after every M6 or M98
GCodeModifier myfile.gcode 
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




