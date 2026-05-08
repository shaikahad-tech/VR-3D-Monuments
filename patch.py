import sys

with open('VirtualMuseumVR/VirtualMuseum.html', 'r') as f:
    lines = f.readlines()

start_idx = -1
end_idx = -1

for i, line in enumerate(lines):
    if "const MONUMENT_BUILDERS = {" in line:
        start_idx = i - 3  # Include the comment block above it
    if "function addCylinder(parent, mat, r, h, pos, rot){" in line:
        # Find the end of this function
        for j in range(i, len(lines)):
            if "parent.add(m); return m;" in lines[j] and "}" in lines[j+1]:
                end_idx = j + 1
                break

if start_idx != -1 and end_idx != -1:
    print(f"Found block from {start_idx} to {end_idx}")
    new_lines = lines[:start_idx] + ["import { MONUMENT_BUILDERS } from './js/monument-builders.js';\n\n"] + lines[end_idx+1:]
    with open('VirtualMuseumVR/VirtualMuseum.html', 'w') as f:
        f.writelines(new_lines)
    print("Patched successfully")
else:
    print(f"Failed to find block. start={start_idx}, end={end_idx}")
