from PIL import Image, ImageDraw
import numpy
import math

IMAGE_WIDTH = 1000
IMAGE_HEIGHT = 1000
ENEMIES_PER_SAMPLE = 1000
RARITY_COLOR = {-1: "black",
                0: "white",
                1: "blue",
                2: "aqua",
                3: "green",
                4: "lime",
                5: "yellow",
                6: "orange",
                7: "red",
                8: "fuchsia"
                }


def random_int(min_val, max_val):
    return min(int(numpy.random.uniform(min_val, max_val+1)), max_val)


def rarity_of_loot(rarity_seed):
    trait_quantity = random_int(0, rarity_seed)

    maximum_trait_efficacy = 0
    for i in range (0, trait_quantity):
        trait_efficacy = random_int(0, rarity_seed)
        maximum_trait_efficacy = max(maximum_trait_efficacy, trait_efficacy)

    return max(maximum_trait_efficacy, trait_quantity)


def kill_enemy(enemy_level):
    rarity_seed = 15*float(enemy_level)/float(IMAGE_WIDTH)
    rarity_seed += numpy.random.normal(0, 1)
    rarity_seed = int(round(numpy.clip(rarity_seed, 0.0, 8.0)))
    drop_quantity = random_int(0, rarity_seed)

    maximum_rarity = -1
    for i in range(0, drop_quantity):
        loot_rarity = rarity_of_loot(rarity_seed)
        maximum_rarity = max(maximum_rarity, loot_rarity)

    return maximum_rarity


def maximum_rarity_outcomes(enemy_level):
    outcomes = [0]*10
    for i in range(0, ENEMIES_PER_SAMPLE):
        outcomes[kill_enemy(enemy_level)]+=1

    probabilities = [0]*10
    for i in range(0, len(outcomes)):
        probabilities[i] = float(outcomes[i])/float(ENEMIES_PER_SAMPLE)
    return probabilities


def draw_graph(image):
    draw = ImageDraw.Draw(image)
    for enemy_level in range(0, IMAGE_WIDTH):
        print (str(enemy_level)+"/"+str(IMAGE_WIDTH))
        probabilities = maximum_rarity_outcomes(enemy_level)
        accumulator = 0
        for rarity in range (-1, 9):
            prob = int(probabilities[rarity]*IMAGE_HEIGHT)
            draw.rectangle(((enemy_level, IMAGE_HEIGHT - accumulator), (enemy_level, IMAGE_HEIGHT-(accumulator + prob))), fill=RARITY_COLOR[rarity])
            accumulator += prob


def main():
    image = Image.new('RGB', (IMAGE_WIDTH, IMAGE_HEIGHT), color='black')
    draw_graph(image)
    image.save('drop_chances.png')


def main2(property_rarities):
    rarity = max(property_rarities+[len(property_rarities)])
    accumulator = 0
    accumulator += rarity**2.0
    accumulator += len(property_rarities)**2.0
    for property_rarity in property_rarities:
        accumulator += property_rarity**2.0
    accumulator *= (1/10.0)
    accumulator **= (1/2.0)
    print math.log10(accumulator**(accumulator**2.8))

main2([0]*0)
main2([1]*1)
main2([2]*2)
main2([3]*3)
main2([4]*4)
main2([5]*5)
main2([6]*6)
main2([7]*7)
main2([8]+[6]*7)
main2([8]*8)