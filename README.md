# Blackbird.io Bitbucket Cloud

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Bitbucket is a cloud-based Git repository hosting service owned by Atlassian that helps developers store and manage their code.

## Before setting up

Before you can connect, please make sure to follow the instructions below. 
Choose the one that matches your desired connection type.

### API Token

Follow the instructions at [this link](https://support.atlassian.com/bitbucket-cloud/docs/create-an-api-token/) to create 
a new API token. Make sure to enable these permissions:
`read:webhook:bitbucket write:webhook:bitbucket delete:webhook:bitbucket read:repository:bitbucket read:user:bitbucket
write:repository:bitbucket read:workspace:bitbucket read:pullrequest:bitbucket write:pullrequest:bitbucket`

After the token is created, copy it and store it in a safe place. You won't be able to access it again.

You also need to copy your account email to create a new connection. 
You can find it in the top-right corner when you click on your avatar.

![Email](image/README/email.png)

### OAuth2

Follow [this guide](https://support.atlassian.com/bitbucket-cloud/docs/use-oauth-on-bitbucket-cloud/) to create
a new OAuth app. Additionally:
- Enter this callback URL: `https://bridge.blackbird.io/api/AuthorizationCode`
- Make sure the `This is a private client` box is checked
- Enable these permissions: `repository workspace repository:write pullrequest pullrequest:write account webhook`

After the app is created, copy the client ID and client secret. 
Store these values in a safe place - you won't be able to access them again.

### Repository access token

Use [this link](https://support.atlassian.com/bitbucket-cloud/docs/create-a-repository-access-token/) to create
a new repository access token. 
After the token is created, copy it and store it in a safe place. You won't be able to access it again.

Make sure to copy your workspace and repository names - you'll need to enter them when setting up the connection.

## Connecting

Navigate to apps and search for Bitbucket. Click _Add connection_ and name your connection for future reference e.g. 'My Bitbucket connection'.

### API Token

1. Select the _API Token_ connection type from the dropdown.
2. Paste your API token and account email.
3. Click _Connect_.

![API Token connection](image/README/api_token.png)

### OAuth2

1. Select the _OAuth2_ connection type from the dropdown.
2. Paste your client ID and client secret.
3. Click _Authorize connection_ and follow Bitbucket's instructions to connect to your custom OAuth2 app.
4. When the process is complete, return to Blackbird and confirm the connection was created successfully.

![OAuth2 connection](image/README/oauth2.png)

### Repository access token

1. Select the _Repository access token_ connection type from the dropdown.
2. Fill in the workspace and repository name inputs. You can use either UUIDs (wrapped in curly brackets) or plain names.
3. Click _Connect_.

![Repository access token connection](image/README/repo_access_token.png)

## Actions

### Branches

- **Search branches**. Search branches in a specific workspace.
- **Branch exists**. Check if branch exists by name.
- **Create branch**. Create a new branch in a repository.

### Files

- **Download file**. Download a file.
- **Download repository as zip**. Download repository content as a zip file.
- **Delete file**. Commit file deletion.
- **File exists**. Check if file exists by path.
- **Upload file**. Commit file upload. Overwrites existing file.
- **Search files in folder**. Search files in a folder.

### Pull requests

- **Create pull request**. Create a new pull request.
- **Merge pull request**. Merge a pull request.

### Repository

- **Search repositories**. Search repositories in a workspace.
- **Get repository**. Get information about a repository.

### Users

> **Note**: These actions are not supported when using the 'Repository access token' connection type 
due to the limited scopes these tokens have.

- **Get my user**. Get information about my user.
- **Get user**. Get information for a specific user.

## Events

### Files

- **On files added**. Triggers for each new file added to the repository.
- **On files added or modified**. Triggers for each new file added to the repository or when an existing file is modified.
- **On files modified**. Triggers when an existing file is modified.
- **On files removed**. Triggers when an existing file is removed.

File path pattern examples:

- `^locales/([a-zA-Z]{2}-[a-zA-Z]{2})\.json$` matches files like `locales/en-US.json` and outputs `en-US` as the extracted part.
- `^locales/en-US.json$` matches only the `locales/en-US.json` file and leaves the extracted part empty.

### Pull requests

- **On pull request created**. Triggers when a pull request is created.
- **On pull request created or updated**. Triggers when a pull request is created or updated.

The `Affected files` output is populated only when `File path patterns` is provided.

## Demo video

[In this demo](https://www.youtube.com/watch?v=1c7Qg4_7pXQ), see how Blackbird.io automates localization workflows directly from Bitbucket repositories.

We’ll walk through a simple but powerful Git-based translation process where content updates in Bitbucket automatically trigger machine translation workflows using DeepL, then commit translated content back into the repository.

In this demo you’ll see how to:

- Detect new or modified content in Bitbucket
- Automatically download and process localization files
- Send content to DeepL for machine translation
- Commit translated content back into Bitbucket
- Configure multilingual workflows using variables and loops
- Dynamically manage folders, filenames, and commit messages
- Extend workflows with AI, PR automation, Jira, Confluence, Notion, and more

This demo highlights how Blackbird.io can orchestrate localization and developer workflows across repositories, AI tools, TMS platforms, and enterprise systems — all in a flexible visual automation platform.

[![Watch the video](https://img.youtube.com/vi/1c7Qg4_7pXQ/0.jpg)](https://www.youtube.com/watch?v=1c7Qg4_7pXQ)

## Missing features

Workspace and project access tokens are not supported in this app. Let us know if you'd like us to add them.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
