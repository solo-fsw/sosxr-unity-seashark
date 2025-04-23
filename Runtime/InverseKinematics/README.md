## EasyIK - A fast and simple Inverse Kinematics solver (FABRIK) for Unity.
From: https://github.com/joaen/EasyIK

![easyIK](https://github.com/joaen/EasyIK/assets/6629861/0b5c82f5-f564-47ae-805e-99c90821ee4a)

Showcase video: https://youtu.be/rYnmHs97CVI

## Setup
1. Drag and drop the script on the root joint.
2. Set the number of joints that will be affected.
3. Assign a transform that will be used as the IK Target.

## Settings
**Pole Target:** The *Pole target* is used to constraint the rotation of the elbow/knee joint. **(Pole Target is only compatible with 3-joint IK chains).**

**Iterations:** How many times the IK solver is allowed to iterate through the chain to reach the target. The iteration will stop if the target is reached or if the maximum number of iterations is reached.

**Tolerance:** Margin of error between the positions of the end joint and IK Target. If the *Tolerance* is set to 2, the target will be reached when the end joint is 2 units away from the target. If the *Tolerance* is 0, the target will be reached when end joint is 0 units away from the target.

**Debug:** Show or hide visual debugging gizmos for joints, rotations, pole, etc. 

## Version
- Unity 2019.4.34f1 LTS
- Unity 2020.3.41f1 LTS
- Unity 2021.3.11f1 LTS

## License
See the [LICENSE](https://github.com/joaen/EasyIK/blob/master/LICENSE) file.
