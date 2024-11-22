from flask import Flask
import secrets
def create_app():
    app = Flask(__name__,template_folder=r'C:\disease_similianity\templates',static_folder=r'C:\disease_similianity\static')
    app.secret_key = secrets.token_hex(16) 

    from .routes import main
    app.register_blueprint(main)

    return app
