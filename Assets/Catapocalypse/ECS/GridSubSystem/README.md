# Grid Sub-System
## Purpose
Create grids with varying size, cell size and position. This will be the building block for the gas and noise system.
## Key components
| Name            | Purpose                                                                                              | Written By                             | Read By                                 |
|-----------------|------------------------------------------------------------------------------------------------------|----------------------------------------|-----------------------------------------|
| `Grid`          | Tag to identify Grid entities                                                                        | -                                      | `SpawnGridSystem`, `CleanupGridSystem`  |
| `GridData`      | Holds grid's settings such <br>as `Size:int2` and `CellSize:float`                                   | -                                      | `SpawnGridSystem`, `CleanupGridSystem`<sup>(1)</sup> |
| `CellReference` | Buffer elements that holds <br/>valuable information about each cell<br/> as well as the cell entity | `SpawnGridSystem`, `CleanupGridSystem` | `SpawnGridSystem`, `CleanupGridSystem`  |
(1) - Only reads the changed state of this component.
## Systems Overview
- **Cleanup Grid System** - Cleans up grid's which data has been altered for reconstruction.
- **Spawn Grid System** - Spawns the grid's cells which data has been altered with the new values.
## Considerations
- Currently the system doesn't save the state of the altered GridData's