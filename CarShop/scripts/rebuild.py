import subprocess

import yaml

with open("../docker-compose.yml", 'r') as stream:
    try:
        doc = yaml.load(stream, Loader=yaml.FullLoader)
    except yaml.YAMLError as exc:
        print(exc)

    services = []
    for index, service in enumerate(doc["services"]):
        print(f'[{index + 1}] {service}')
        services.append(service)

    print()

    while True:
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
        break


