from PIL import Image, ImageDraw
import glob
import os

class Size:
    def __init__(self, w, h) -> None:
        self.w = int(w)
        self.h = int(h)


class OutputConfig:
    w = 600
    h = 400
    conner_radius = 50
    input_dir = "temp"
    output_dir = "output"
    
    def get_wh(self):
        return self.w / self.h
    
    def calc_crop_wh(self, old_w, old_h):
        old_wh_rate = old_w / old_h
        if old_wh_rate > self.get_wh():
            return Size(old_w / (old_wh_rate / self.get_wh()), old_h)
        else:
            return Size(old_w, old_h * (old_wh_rate / self.get_wh()))


def add_corners(im, rad):
    circle = Image.new('L', (rad * 2, rad * 2), 0)
    draw = ImageDraw.Draw(circle)
    draw.ellipse((0, 0, rad * 2, rad * 2), fill=255)
    alpha = Image.new('L', im.size, 255)
    w, h = im.size
    alpha.paste(circle.crop((0, 0, rad, rad)), (0, 0))
    alpha.paste(circle.crop((0, rad, rad, rad * 2)), (0, h - rad))
    alpha.paste(circle.crop((rad, 0, rad * 2, rad)), (w - rad, 0))
    alpha.paste(circle.crop((rad, rad, rad * 2, rad * 2)), (w - rad, h - rad))
    im.putalpha(alpha)
    return im


def trim_image(path, config: OutputConfig):
    im = Image.open(path)

    center_x = im.width / 2.0
    center_y = im.height / 2.0
    
    crop_size = config.calc_crop_wh(im.width, im.height)
    
    # print(im.size)
    # print(new_size.w, new_size.h)
    
    cropped = im.crop((
        center_x - crop_size.w/2, center_y - crop_size.h/2,
        center_x + crop_size.w/2, center_y + crop_size.h/2))
    
    resized = cropped.resize((config.w, config.h), Image.LANCZOS)

    added_conner = add_corners(resized, config.conner_radius)
    
    added_conner.save(f"./{config.output_dir}/{os.path.basename(path)}")



def main():
    config = OutputConfig()

    files = glob.glob(f"./{config.input_dir}/*.png")
    for file in files:
        print(f"trim: {file}")
        trim_image(file, config)
main()