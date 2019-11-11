#!/usr/bin/env python

from aioconsole import ainput
import asyncio
import json
import websockets
import sys


uri = "ws://127.0.0.1:8765"


async def read_only_connection():
    async with websockets.connect(uri) as websocket:
        while True:
            data = await websocket.recv()
            print(f"< {data}")


async def write_only_connection():
    async with websockets.connect(uri) as websocket:
        while True:
            block_id = await ainput("send goal to block id? ")
            goal = await ainput("\tgoal loc: ")
            output_msg = {
                "type": "move_goal",
                "data": {
                    "block_id": int(block_id),
                    "goal": [int(x) for x in goal.strip().split(",")]
                }
            }
            await websocket.send(json.dumps(output_msg))


async def main():
    if len(sys.argv) < 2:
        print("Usage: client.py read|write")
        return

    if sys.argv[1] == 'read':
        await read_only_connection()
    elif sys.argv[1] == 'write':
        await write_only_connection()
    else:
        print("Usage: client.py read|write")
    

asyncio.run(main())