Assembly Reference
Path to assembly for binaries are defined in CS.props and user_local.props which can be found at $(SolutionDirectory)\Config
user_local.props defines path to binaries found in the bin folder of the local Dynamo repository
If the specified binary is not found, the path to the nuget packages binaries will be used instead which is defined in the CS.props file