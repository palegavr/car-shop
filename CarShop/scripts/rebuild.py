import os
import subprocess

import yaml


def cls():
    os.system('cls' if os.name == 'nt' else 'clear')


def print_services(_services):
    for _index, _service in enumerate(_services):
        print(f'[{_index + 1}] {_service}')


with open("../docker-compose.yml", 'r') as stream:
    try:
        doc = yaml.load(stream, Loader=yaml.FullLoader)
    except yaml.YAMLError as exc:
        print(exc)

    services = []
    for index, service in enumerate(doc["services"]):
        services.append(service)

    while True:
        print_services(services)
        print()
        print('Чтобы выйти, введите "exit".')
        service_index = input('Введите номер сервиса, который нужно пересобрать: ')
        if service_index == 'exit':
            break

        if not service_index.isdigit():
            continue

        service_index = int(service_index)

        if service_index < 1 or service_index > len(services):
            continue

        subprocess.call(['docker-compose', 'up', '--build', '-d', services[service_index - 1]])
        cls()
