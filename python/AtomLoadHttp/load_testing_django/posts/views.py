from django.http import HttpRequest, HttpResponse
from django.views.generic import View

from .models import Post


class TestView(View):
    def get(self, request: HttpRequest):
        name: str = request.GET.get('name')
        if name:
            message: str = f'Hello {name}'
        else:
            message: str = f'Hello World'

        return HttpResponse(message)


class PostView(View):
    def get(self, request: HttpRequest):
        posts = Post.objects.all()

        return HttpResponse(f'{posts}')
