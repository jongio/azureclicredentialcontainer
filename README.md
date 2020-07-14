# How to use AzureCliCredential in a Container

[AzureCliCredential](https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/identity/Azure.Identity/src/AzureCliCredential.cs) is a new credential type in [Azure.Identity](https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/identity/Azure.Identity).  It allows your code to use the [Azure CLI](https://aka.ms/azcliget) to generate bearer tokens to be used by other Azure SDK clients.

For example, the following code news up an `AzureCliCredential` and passes it to the Azure Key Vault `KeyClient`, which in turn will call the `GetToken` method on the `AzureCliCredential` class when the first method is called that requires an AAD token.

```csharp
var cred = new AzureCliCredential();
var client = new KeyClient(new Uri("https://jongkv.vault.azure.net"), cred);
var key = await client.GetKeyAsync("key1");
```

If you want to run this code in a container, then you need to install the Azure CLI and mount a volume to your `${HOME}/.azure` folder for Linux and `${USERPROFILE}/.azure` folder for Windows.

## Azure CLI Setup

1. Install the [Azure CLI](https://aka.ms/azcliget)
2. Run `az login` in the same host OS that you will use for development.  So, if you use WSL2, then run this in a WSL2 terminal. If you use Git Bash, then run this there. The Azure CLI will cache tokens locally in `${HOME}/.azure` that will be used by AzureCliCredential.

## Dockerfile
Here's how you install the Azure CLI with one line of Dockerfile code:

`RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash`

See [Dockerfile](src/net/Dockerfile) for full code.

> This example is for .NET and Linux only.  If you want to see this working in other langs or Windows, then please file an issue.

## docker-compose.yml

If you only install the Azure CLI, you will be required to `az login` in the container.  To have the container honor your host machines `az login`, then you need to expose the `.azure` folder on your host to your container.

Here's how to do that in `docker-compose.yml`

Linux:
```
volumes: 
   - "${HOME}/.azure:/root/.azure"
```

Windows:
```
volumes: 
   - "${USERPROFILE}/.azure:/root/.azure"
```

See [docker-compose.yml](src/net/docker-compose.yml) for full code Linux, and [docker-compose.windows.yml](src/net/docker-compose.windows.yml) for full code Windows.


## docker-compose up --build

### Linux
Run `docker-compose up --build` your `AzureCliCredential` code will now work.

### Windows
Windows handles the user's home directory differently than Linux, so you need to use ${USERPROFILE} instead of ${HOME} in your docker-compose call.

Run `docker-compose -f docker-compose.windows.yml up --build` your `AzureCliCredential` code will now work.


## Running AzureCliCredential in Kubernetes

I'm running Docker Desktop and WSL2. The following is for that configuration. If you are using a different setup and can't get this to work, then let me know and I should be able to help you get it all setup.

Standard Kubernetes hostPath based volume mounts do not currently work with Docker Desktop and WSL2, so you need to do the following:

Create a directory in the /mnt/wsl folder to mount to, then mount from ${HOME}/.azure to that folder.  I don't exactly know why this is required, but it works.  Here's more info if you are interested in researching it: [Kubernetes Volumes not correctly mounted with WSL2](https://github.com/docker/for-win/issues/5325#issuecomment-645859528)

```bash
mkdir /mnt/wsl/.azure
sudo mount --bind ${HOME}/.azure /mnt/wsl/.azure
```

Then in your Kubernetes config file you specify the mount path like this:

```yaml
volumes:
   - hostPath:
      path: /run/desktop/mnt/host/wsl/.azure
      type: Directory
      name: cli
```

You can find the entire file example here: [.k8s/k8s.yml](src/net/.k8s/k8s.yml)

Then run `kubeclt apply -f .k8s` and you will see `key1` outputed to your logs.

You can remove the mount with the following:

```bash
sudo umount /mnt/wsl/.azure
```