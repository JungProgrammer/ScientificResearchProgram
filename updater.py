from flask import Flask, redirect, send_from_directory, abort
import os
import waitress

app = Flask(__name__)


@app.route('/download_update/<version>')
def get_new_version(version):
    if os.path.exists(f'{version}.zip'):
        print(version)
        return send_from_directory(f'', f'{version}.zip', as_attachment=True)
    else:
        return abort(404)


@app.route('/get_updater')
def get_updater():
    return send_from_directory(f'', f'updater.zip', as_attachment=True)


@app.route('/update')
def update_test():
    return send_from_directory(f'publish', f'publish.htm')


@app.route('/setup.exe')
def setup():
    return send_from_directory(f'publish', f'setup.exe', as_attachment=True)


@app.route('/update/<path:filename>')
def update_test_file(filename: str):
    print(filename)
    return send_from_directory(f'publish', f'{filename}', as_attachment=True)


waitress.serve(app, port=8000)
