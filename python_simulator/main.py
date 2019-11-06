#!/usr/bin/env python

import asyncio
import json

from block_simulator import BlockSimulator
from socket_server import SocketServer


sim = None
comm = None 


# callback when data receieved from the client
# move goals are sent to the sim
def on_receive(message, client_address):
    print(f'> {client_address[0]}:{client_address[1]} = {message}')
    
    message = json.loads(message)
    if message['type'] == 'move_goal':
        block_id = message['data']['block_id']
        goal = message['data']['goal']
        sim.move_block(block_id, goal)


# callback when connection with client established
# return str to send back to client
def on_connect(server_address):
    output_msg = {
        "type": "connection",
        "data": {
            "status": "connected",
            "address": f"{server_address[0]}:{server_address[1]}"
        }
    }
    return json.dumps(output_msg)


# callback when simulator goal is comeplete
# sends result to client
def on_complete(task_id, block, success, err_msg):
    if not success:
        print(f'Task:{task_id} to move block#{block} failed: ', err_msg)
    else:
        print(f'Task:{task_id} to move block#{block} succeeded')
    output_msg = {
        "type": "move_goal_complete",
        "data": {
            "task_id": task_id,
            "success": success,
            "err_msg": err_msg
        }
    }
    comm.send_data(json.dumps(output_msg))


async def main():
    global sim
    global comm
    
    sim = BlockSimulator('world1.json', on_complete)
    comm = await SocketServer.create(on_receive=on_receive, on_connect=on_connect)
    
    while True:
        # get game state and publish as json
        output_msg = {
            "type": "game_state",
            "data": [block.get_object() for block in sim.blocks]
        }
        comm.send_data(json.dumps(output_msg))
        await asyncio.sleep(0.1)


if __name__ == '__main__':
    asyncio.run(main())