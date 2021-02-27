from django.urls import path
from .views import TestView, PostView

urlpatterns = [
    path('test/', TestView.as_view()),
    path('posts/', PostView.as_view())
]
