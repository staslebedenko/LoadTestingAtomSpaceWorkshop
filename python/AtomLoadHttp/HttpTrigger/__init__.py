import sys

import azure.functions as func
from azf_wsgi import AzureFunctionsWsgi

sys.path.insert(0, './load_testing_django')
from load_testing_django.wsgi import application


def main(req: func.HttpRequest, context: func.Context) -> func.HttpResponse:
    return AzureFunctionsWsgi(application).main(req, context)
