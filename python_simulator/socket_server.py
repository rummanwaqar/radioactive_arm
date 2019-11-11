#!/usr/bin/env python

import asyncio
import random
import time
import websockets


class SocketServer(object):
    def __init__(self, address, port, on_connect, on_receive):
        # start websocket server
        self.server = websockets.serve(self._websocket_handler, address, port)
        # websocket clients
        self._clients = set()
        
        # output data queue
        self._output_queue = asyncio.Queue()
        # output coroutine
        self._output_task = asyncio.create_task(self._run_output())

        # on connect callback
        self._on_connect = on_connect
        # on data receive callback
        self._on_receive = on_receive

    def send_data(self, data: str):
        if type(data) != str:
            raise TypeError("Data should be of type string")
        self._output_queue.put_nowait(data)

    async def _run_output(self):
        while True:
            data = await self._output_queue.get()
            for client in self._clients:
                if client.open:
                    await client.send(data)
                else:
                    print("error: client is closed")
            self._output_queue.task_done()

    async def _websocket_handler(self, websocket, path):
        print(f'Client connected: {websocket.remote_address[0]}:{websocket.remote_address[1]}')
        if self._on_connect is not None:
            await websocket.send(self._on_connect(websocket.local_address))
        self._clients.add(websocket)
        try:
            async for message in websocket:
                if self._on_receive is not None:
                    self._on_receive(message, websocket.remote_address)
        except Exception:
            pass
        finally:
            self._clients.remove(websocket)
            print(f'Client disconnected: {websocket.remote_address[0]}:{websocket.remote_address[1]}')

    @classmethod
    async def create(cls, address="127.0.0.1", port=8765, on_connect=None, on_receive=None):
        self = cls(address, port, on_connect, on_receive)
        # start the socket server
        await asyncio.gather(self.server)
        return self
            

def on_receive(message, client_address):
    print(f'> {client_address[0]}:{client_address[1]} = {message}')

def on_connect(server_address):
    return f"Connected to {server_address[0]}:{server_address[1]}"

async def main():
    comm = await SocketServer.create(on_receive=on_receive, on_connect=on_connect)

    while True:
        token = random.random()
        comm.send_data(str(token))
        await asyncio.sleep(0.5)

if __name__ == '__main__':
    asyncio.run(main())