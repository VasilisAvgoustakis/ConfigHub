# Before Coding

## Questions to Clarify

- What kind of configurations will this tool manage? (e.g. app settings, db connections, feature toggles?)
- Answer: App Settings, feature toggles, environment-dependent behaviors and db connnections

- What format do you prefer for your configuration files?
- Answer: Both YAML and JSON config files.

- Do you want this configuration to be reloadable at runtime or only load at app startup?
- Answer: Only at app start up!

- Where will configurations be stored?
- Answer: Local File System but I would like to keep the possibility of integrating a cloud based db in the future open for implementation.

- Do you have a preferred Version control?
- Answer: Git & Github
