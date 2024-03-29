# Load Testing Azure infrastructure Workshop(C# and Python). For IT2School and AtomSpace.

- The goal of this workshop is to create

- Load testing of serverless applications in Azure.

- Lets start with a database stuff and real C# solution.

- Lets take a sample application and take a look.

- Lets add Polly to the mix.

- Lets create additional storage queue for error messages.

- Lets create additional function to restore failed requests.

- Lets deploy application to Azure and add allowed IP addresses of application to firewall and yours IP too.

- Lets try a query to our application, does it work?

# Python setup

Install command line components:  
https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#v2  
https://docs.microsoft.com/en-us/cli/azure/install-azure-cli

Create application:

```powershell
func init AtomLoadHttp --python 
```

or alternatively:

```powershell
# Initialize project.
func init AtomLoadHttp --worker-runtime python --docker

# Open directory.
cd AtomLoadHttp

# Create HTTP trigger.
func new --name HttpTrigger --template "HTTP trigger" --authlevel "anonymous"

# Create virtualenv and use it. 
python -m venv .venv
# Windows.
& .venv/Scripts/activate
# Or linux.
source .venv/Scripts/activate
```

Then we need to deploy a created app to Azure.

```powershell
# Azure login.
az login

# Pulish created app.
func azure functionapp publish <APP_NAME>
```
