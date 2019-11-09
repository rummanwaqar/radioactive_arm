#!/usr/bin/env python

import asyncio
from enum import Enum
import json
import numpy as np


class BlockType(Enum):
    cube0 = 0
    cube1 = 1
    cube2 = 2


class Block(object):
    # class variable
    id = 0

    def __init__(self, obj_type: BlockType, position: list):
        self.id = Block.id
        Block.id += 1

        self.type = obj_type
        self.position = np.float64(position)
    
    def get_object(self):
        return {
            "id": self.id,
            "type": self.type.name,
            "position": self.position.tolist(),
        }

    def __str__(self):
        return str(self.get_object())

    def __repr__(self):
        return self.__str__()


class MoveTask(object):
    '''
    manages move task with linear interpolation between two points 
    '''

    id = 0
    def __init__(self, block, final_pos, dist_per_step):
        self.id = MoveTask.id
        MoveTask.id += 1

        self.block = block
        self.final = np.float64(final_pos)
        self._delta_d = dist_per_step

        # unit direction vector to move along
        self._u = self.final - np.float64(block.position)
        self._u /= np.linalg.norm(self._u)

    def move(self):
        '''
        moves one step
        returns True when at goal
        '''
        if np.array_equal(self.block.position, self.final):
            return True
        
        if abs(np.linalg.norm(self.final - self.block.position)) >= self._delta_d:
            self.block.position += self._delta_d * self._u
            return False
        else:
            self.block.position = self.final
            return True


class BlockSimulator(object):
    def __init__(self, world_file, on_complete=None):
        # world state
        self.grid_size = [0,0]
        self.blocks = []
        self._load_world(world_file)

        # main simaultor coroutine
        self._simulator_task = asyncio.create_task(self._run())
        # goal queue
        self._goal_queue = asyncio.Queue()
        # goal callback
        self._on_complete = on_complete

        # simulator state
        self._move_speed = 0.5
        self._time_step = 0.1
        self._current_goal = None

    def get_state(self):
        return "state"

    def move_block(self, block_id, goal):
        # find block
        block = self._get_block_by_id(block_id)
        if block is None:
            raise RuntimeWarning(f'block: {block_id} not found')

        self._goal_queue.put_nowait(MoveTask(block, goal, self._move_speed*self._time_step))

    async def _run(self):
        '''
        move cubes 
        '''
        while True:
            # wait for goal and validate goal
            if self._current_goal is None:
                self._current_goal = await self._goal_queue.get()
                print(f'Starting task: {self._current_goal.id}')

                # check if block is movable (nothing should be above it)
                if not self._is_block_movable(self._current_goal.block):
                    self._on_complete(self._current_goal.id, self._current_goal.block.id, False, 'Block is not movable')
                    self._goal_queue.task_done()
                    self._current_goal = None
                # check if goal position is valid
                elif not self._is_goal_valid(self._current_goal.final):
                    self._on_complete(self._current_goal.id, self._current_goal.block.id, False, 'Goal is not valid')
                    self._goal_queue.task_done()
                    self._current_goal = None

            if self._current_goal is not None:
                await asyncio.sleep(self._time_step)
                if self._current_goal.move():
                    self._on_complete(self._current_goal.id, self._current_goal.block.id, True, '')
                    self._current_goal = None
                    self._goal_queue.task_done()

    def _is_block_movable(self, block):
        position_above = block.position + np.array([0,0,1])
        return self._get_block_by_pos(position_above) is None

    def _is_goal_valid(self, goal):
        # check bounds
        within_x_axis = (goal[0] > -self.grid_size[0] / 2) and (goal[0] < self.grid_size[0] / 2)
        within_y_axis = (goal[1] >= 0) and (goal[1] < self.grid_size[1])
        
        # check is space is empty
        space_empty = self._get_block_by_pos(goal) is None

        # check if there is a surface below (grid or another block)
        position_below = goal - np.array([0,0,1])
        has_surface = position_below[2] == -1 or self._get_block_by_pos(position_below) is not None

        return within_x_axis and within_y_axis and space_empty and has_surface

    def _load_world(self, url):
        world_json = []
        with open(url, 'r') as f: 
            world_json = json.load(f)
        self.grid_size = world_json['grid']
        for obj_json in world_json['objects']:
            self.blocks.append(Block(BlockType[obj_json['type']], obj_json['position']))

    def _get_block_by_id(self, id):
        for block in self.blocks:
            if block.id == id:
                return block
        return None

    def _get_block_by_pos(self, pos):
        for block in self.blocks:
            if np.array_equal(block.position, pos):
                return block
        return None


def on_complete(task_id, block, success, err_msg):
    if not success:
        print(f'Task:{task_id} to move block#{block} failed: ', err_msg)
    else:
        print(f'Task:{task_id} to move block#{block} succeeded')


async def main():
    sim = BlockSimulator('world1.json', on_complete)

    sim.move_block(0, [1, 2, 1])
    sim.move_block(2, [3, 2, 1])
    sim.move_block(1, [1, 5, 2])
    sim.move_block(1, [3, 2, 2])
    
    while True:
        await asyncio.sleep(0.1)
        for block in sim.blocks:
            print(block)
        print('---')


if __name__ == '__main__':
    asyncio.run(main())