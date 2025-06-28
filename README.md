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

## 🧪 Example

```
GCodeModifier myfile.gcode
# Inserts "M8" before first G0 X.. Y.. after every M6 or M98

GCodeModifier myfile.gcode "M8,G4 P1000"
# Inserts "M8" and "G4 P1000" before first G0 X.. Y.. after every M6 or M98
```

## 📒 Notes

For latest MacOS (BigSur Up), in order to run this utility you need to codesign it else you will get error `zsh: killed`

```
mkdir <path-where-you-extracted-the-GCodeUtil>
codesign --force --deep --sign - GCodeUtil
```

## ✍️ Author

Developed by FrancisCreation
Pull requests and contributions welcome!




