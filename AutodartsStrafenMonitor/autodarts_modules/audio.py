
import pygame
import os

class SoundEffects:
    def __init__(self):
        pygame.mixer.init()
        self.sounds = {
            '26': pygame.mixer.Sound(os.path.join('data', 'sounds', '26.wav')),
            '180': pygame.mixer.Sound(os.path.join('data', 'sounds', '180.wav'))
        }
    
    def play_sound(self, score):
        if str(score) in self.sounds:
            self.sounds[str(score)].play()
