package main

import (
	"fmt"
	"io/ioutil"
	"log"
	"strings"
	"os"
	"bufio"
	"net"
	"unicode"
	"strconv"

	"github.com/jroimartin/gocui"
)


var path string = "/users/orionxi/Documents/Go/hello/introspector.conf"



func Write(x string, y string, z string) {


input, err := ioutil.ReadFile(path)
        if err != nil {
                log.Fatalln(err)
        }
        lines := strings.Split(string(input), "\n")
        for i, line := range lines {
                if strings.Contains(line, x) {	
                        lines[i] = y + " = " + z                   
                }
        }
        output := strings.Join(lines, "\n")
        err = ioutil.WriteFile(path, []byte(output), 0644)
        if err != nil {
                log.Fatalln(err)
        }
}


func Read(x string) string {

	if file, err := os.Open(path); err == nil {
		defer file.Close()
		scanner := bufio.NewScanner(file)
		
		for scanner.Scan() {
			line := scanner.Text()
			if strings.Contains(line, x) {
			
				if strings.Contains(x, "LOCAL_END") {
				
				return line
				}
				if strings.Contains(x, "LOCAL_PORT") {
				
				return line
				}
			return ""	
			}			
		}
		if err = scanner.Err(); err != nil {
			log.Fatal(err)
			
			}
		}	else {
			log.Fatal(err)
		}
	
		return ""
}


func ReadFile() string {
	b, err := ioutil.ReadFile(path)
    if err != nil {
        fmt.Print(err)
    }
    str := string(b) 
    
    return str
}

func RemoveSpace(str string) string {
	return strings.Map(func(r rune) rune {
			if unicode.IsSpace(r) {
				return -1
			}
			return r
		}, str)
}

func nextView(g *gocui.Gui, v *gocui.View) error {
	if v == nil || v.Name() == "side" {
		return g.SetCurrentView("file")
	}
	return g.SetCurrentView("side")
}

func nextView2(g *gocui.Gui, v *gocui.View) error {
	if v == nil || v.Name() == "hostinput" {
		return g.SetCurrentView("port")
	}
	return g.SetCurrentView("clear")
}

func nextView3(g *gocui.Gui, v *gocui.View) error {
	if v == nil || v.Name() == "clear" {
		return g.SetCurrentView("hostinput")
	}
	return g.SetCurrentView("hostinput")
}

func cursorDown(g *gocui.Gui, v *gocui.View) error {
	if v != nil {
		cx, cy := v.Cursor()
		if err := v.SetCursor(cx, cy+1); err != nil {
			ox, oy := v.Origin()
			if err := v.SetOrigin(ox, oy+1); err != nil {
				return err
			}
		}
	}
	return nil
}

func cursorUp(g *gocui.Gui, v *gocui.View) error {
	if v != nil {
		ox, oy := v.Origin()
		cx, cy := v.Cursor()
		if err := v.SetCursor(cx, cy-1); err != nil && oy > 0 {
			if err := v.SetOrigin(ox, oy-1); err != nil {
				return err
			}
		}
	}
	return nil
}

func Display(g *gocui.Gui, v *gocui.View) error {
		var l string
		var err error

		_, cy := v.Cursor()
		if l, err = v.Line(cy); err != nil {
			l = ""
			}

		maxX, maxY := g.Size()
		
		if l == "Display Host IP Address:"  {
	if v, err := g.SetView("msg", 50, 5, maxX/2, maxY/2-15); err != nil {
			if err != gocui.ErrUnknownView {
				return err
			}
			v.Title = "Host IP Address"
			fmt.Fprintln(v, Read("LOCAL_END"))	
	if err := g.SetCurrentView("msg"); err != nil {
			return err
				}
			}
		}
		if l == "Display Host Port Number Used:" {
	if v, err := g.SetView("msg", 50, 5, maxX/2, maxY/2-15); err != nil {
			if err != gocui.ErrUnknownView {
				return err
			}
			v.Title = "Host Port Number"
			fmt.Fprintln(v, Read("LOCAL_PORT"))	
	if err := g.SetCurrentView("msg"); err != nil {
			return err
				}
			}
		}
		if l == "Edit Host IP Address:" {
		
	if v, err := g.SetView("input", 50, 5, maxX/2, maxY/2-20); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		v.Title = "Input IP Address"
		fmt.Fprintln(v, "")
	if err := g.SetCurrentView("input"); err != nil {
			return err
		}
		v.Editable = true
		v.Wrap = true

		}	
	}
		if l == "Edit Host Port Number:" {
		
	if v, err := g.SetView("portinput", 50, 5, maxX/2, maxY/2-20); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		v.Title = "Input Port Number"
		fmt.Fprintln(v, "")
	if err := g.SetCurrentView("portinput"); err != nil {
			return err
		}
		v.Editable = true
		v.Wrap = true
		}
		
	}
	
	if l == "Edit Host IP Address & Port Number:" {
	
	if v, err := g.SetView("iplabel", 50, 5, 75, 7); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		fmt.Fprintln(v,"Input Host IP Address:")
		v.Frame = true
	}
	if v, err := g.SetView("hostinput", 75, 5, 110, 7); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		fmt.Fprintln(v, "")
		v.Editable = true
		v.Wrap = true
		v.Frame = true
	
		}	
		
	if v, err := g.SetView("portlabel", 50, 7, 75, 9); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		fmt.Fprintln(v,"Input Host Port Number:")
		v.Frame = true
	}
	if v, err := g.SetView("port", 75, 7, 110, 9); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		fmt.Fprintln(v, "")
		v.Editable = true
		v.Wrap = true
		v.Frame = true
		
		}
	
	if v, err := g.SetView("clear", 65, 9, 85, 11); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		fmt.Fprintln(v,"Close")
		v.Wrap = true
		v.Highlight = true
		v.Frame = true
		
		}
		
	if err := g.SetCurrentView("hostinput"); err != nil {
			return err
		}
	}
		
	return nil
}


func DisplayQuit(g *gocui.Gui, v *gocui.View) error {
		var l string
		var err error

		_, cy := v.Cursor()
		if l, err = v.Line(cy); err != nil {
			l = ""
			}
	
	if l == "YES" {
		return gocui.ErrQuit
	}
	if l == "NO" {
	if err := g.DeleteView("close"); err != nil {
		return err
		}
	if err := g.SetCurrentView("side"); err != nil {
		return err
		}
	}
	return nil
}


func DisplayFile(g *gocui.Gui, v *gocui.View) error {
		var l string
		var err error

		_, cy := v.Cursor()
		if l, err = v.Line(cy); err != nil {
			l = ""
			}
			
	if l == "Show Configuration File:" {
	if v, err := g.SetView("msg", 50, 10, 100, 50); err != nil {
			if err != gocui.ErrUnknownView {
			return err
		}
			fmt.Fprintln(v, ReadFile())
	if err := g.SetCurrentView("msg"); err != nil {
			return err
			}
		}
	}
	return nil
}



func EditIPaddress(g *gocui.Gui, v *gocui.View) error {
	vb := v.ViewBuffer()
	
	trial := net.ParseIP(RemoveSpace(vb))
	if trial.To4() == nil {
			maxX, maxY := g.Size()
			if v, err := g.SetView("errmsg", 50, 5, maxX/2, maxY/2-20); err != nil {
					if err != gocui.ErrUnknownView {
					return err
					}
				v.Title = "INVALID IP ADDRESS!!!!!!"
				fmt.Fprintln(v, "Please Select Another")
		}
	} else {
	
		Write("LOCAL_END", "LOCAL_ENDPOINT", RemoveSpace(vb))
		
		maxX, maxY := g.Size()
			if v, err := g.SetView("errmsg", 50, 5, maxX/2, maxY/2-18); err != nil {
					if err != gocui.ErrUnknownView {
					return err
					}	
				v.Title = "Host IP Address and Port Number"
				fmt.Fprintln(v, Read("LOCAL_END"))
				fmt.Fprintln(v, Read("LOCAL_PORT"))
		}
	}
	
	if err := g.DeleteView("hostinput"); err != nil {
		return err
	}
	if err := g.DeleteView("iplabel"); err != nil {
		return err
	}
	if err := g.DeleteView("port"); err != nil {
		return err
	}
	if err := g.DeleteView("portlabel"); err != nil {
		return err
	}
	if err := g.DeleteView("clear"); err != nil {
		return err
	}
	if err := g.SetCurrentView("errmsg"); err != nil {
		return err
	}
	return nil
}


func EditIPaddress2(g *gocui.Gui, v *gocui.View) error {
	vb := v.ViewBuffer()
	
	trial := net.ParseIP(RemoveSpace(vb))
	if trial.To4() == nil {
			maxX, maxY := g.Size()
			if v, err := g.SetView("errmsg", 50, 5, maxX/2, maxY/2-20); err != nil {
					if err != gocui.ErrUnknownView {
					return err
					}
				v.Title = "INVALID IP ADDRESS!!!!!!"
				fmt.Fprintln(v, "Please Select Another")
				if err := g.SetCurrentView("errmsg"); err != nil {
				return err
			}
		}
	} else {
	
		Write("LOCAL_END", "LOCAL_ENDPOINT", RemoveSpace(vb))
		
		maxX, maxY := g.Size()
			if v, err := g.SetView("errmsg", 50, 5, maxX/2, maxY/2-20); err != nil {
					if err != gocui.ErrUnknownView {
					return err
					}	
				v.Title = "Changed IP Address"
				fmt.Fprintln(v, vb)
				if err := g.SetCurrentView("errmsg"); err != nil {
				return err
			}
		}
	}
	
	if err := g.DeleteView("input"); err != nil {
		return err
	}
	return nil
}



func EditPort(g *gocui.Gui, v *gocui.View) error {
	
	vb := v.ViewBuffer()
	
	if _, err := strconv.Atoi(RemoveSpace(vb)); err == nil && len(RemoveSpace(vb)) == 4 {
			Write("LOCAL_PO", "LOCAL_PORT", RemoveSpace(vb))
			
			maxX, maxY := g.Size()
			if v, err := g.SetView("errmsg", 50, 5, maxX/2, maxY/2-18); err != nil {
					if err != gocui.ErrUnknownView {
					return err
					}
				v.Title = "Host IP Address and Port Number"
				fmt.Fprintln(v, Read("LOCAL_END"))
				fmt.Fprintln(v, Read("LOCAL_PORT"))
		}

	} else {

		maxX, maxY := g.Size()
			if v, err := g.SetView("errmsg", 50, 5, maxX/2, maxY/2-20); err != nil {
					if err != gocui.ErrUnknownView {
					return err
					}			
				v.Title = "INVALID PORT NUMBER!!!!!"
				fmt.Fprintln(v, "Please Select Another")
		}
	}	
		
	if err := g.DeleteView("hostinput"); err != nil {
		return err
	}
	if err := g.DeleteView("iplabel"); err != nil {
		return err
	}
	if err := g.DeleteView("port"); err != nil {
		return err
	}
	if err := g.DeleteView("portlabel"); err != nil {
		return err
	}
	if err := g.DeleteView("clear"); err != nil {
		return err
	}
	if err := g.SetCurrentView("errmsg"); err != nil {
		return err
	}
	return nil
}

func EditPort2(g *gocui.Gui, v *gocui.View) error {
	vb := v.ViewBuffer()
	
	if _, err := strconv.Atoi(RemoveSpace(vb)); err == nil && len(RemoveSpace(vb)) == 4 {
			Write("LOCAL_PO", "LOCAL_PORT", RemoveSpace(vb))
			
			maxX, maxY := g.Size()
			if v, err := g.SetView("errmsg", 50, 5, maxX/2, maxY/2-20); err != nil {
					if err != gocui.ErrUnknownView {
					return err
					}
				v.Title = "Changed Port Number"
				fmt.Fprintln(v, vb)
				if err := g.SetCurrentView("errmsg"); err != nil {
				return err
			}
		}
	} else {

		maxX, maxY := g.Size()
			if v, err := g.SetView("errmsg", 50, 5, maxX/2, maxY/2-20); err != nil {
					if err != gocui.ErrUnknownView {
					return err
					}			
				v.Title = "INVALID PORT NUMBER!!!!!"
				fmt.Fprintln(v, "Please Select Another")
				if err := g.SetCurrentView("errmsg"); err != nil {
				return err
			}
		}
	}
	if err := g.DeleteView("portinput"); err != nil {
		return err
	}	
	return nil
}


func delMsg(g *gocui.Gui, v *gocui.View) error {
	if err := g.DeleteView("msg"); err != nil {
		return err
	}
	if err := g.SetCurrentView("side"); err != nil {
		return err
	}
	return nil
}

func delEdit(g *gocui.Gui, v *gocui.View) error {
	if err := g.DeleteView("input"); err != nil {
		return err
	}
	if err := g.SetCurrentView("side"); err != nil {
		return err
	}
	return nil
}


func delError(g *gocui.Gui, v *gocui.View) error {
	if err := g.DeleteView("errmsg"); err != nil {
		return err
	}
	if err := g.SetCurrentView("side"); err != nil {
		return err
	}
	return nil
}

func delPort(g *gocui.Gui, v *gocui.View) error {
	if err := g.DeleteView("portinput"); err != nil {
		return err
	}
	if err := g.SetCurrentView("side"); err != nil {
		return err
	}
	return nil
}

func delQuit(g *gocui.Gui, v *gocui.View) error {
	if err := g.DeleteView("close"); err != nil {
		return err
	}
	if err := g.SetCurrentView("side"); err != nil {
		return err
	}
	return nil
}

func delBoth(g *gocui.Gui, v *gocui.View) error {
	if err := g.DeleteView("hostinput"); err != nil {
		return err
	}
	if err := g.DeleteView("iplabel"); err != nil {
		return err
	}
	if err := g.DeleteView("port"); err != nil {
		return err
	}
	if err := g.DeleteView("portlabel"); err != nil {
		return err
	}
	if err := g.DeleteView("clear"); err != nil {
		return err
	}
	if err := g.SetCurrentView("side"); err != nil {
		return err
	}
	return nil
}

func QuitOption(g *gocui.Gui, v *gocui.View) error {

maxX, maxY := g.Size()
	if v, err := g.SetView("close", 50, 5, maxX/2, maxY/2-20); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		v.Title = "Do You Want To Close The Interface"
		fmt.Fprintln(v, "")
		fmt.Fprintln(v, "YES")
		fmt.Fprintln(v, "NO")
		v.Highlight = true
			if err := g.SetCurrentView("close"); err != nil {
			return err
		}
	}
	return nil
}

func quit(g *gocui.Gui, v *gocui.View) error {
	return gocui.ErrQuit
}

func keybindings(g *gocui.Gui) error {
	if err := g.SetKeybinding("side", gocui.KeyTab, gocui.ModNone, nextView); err != nil {
		return err
	}
	if err := g.SetKeybinding("file", gocui.KeyTab, gocui.ModNone, nextView); err != nil {
		return err
	}
	if err := g.SetKeybinding("hostinput", gocui.KeyTab, gocui.ModNone, nextView2); err != nil {
		return err
	}
	if err := g.SetKeybinding("port", gocui.KeyTab, gocui.ModNone, nextView2); err != nil {
		return err
	}
	if err := g.SetKeybinding("clear", gocui.KeyTab, gocui.ModNone, nextView3); err != nil {
		return err
	}
	if err := g.SetKeybinding("side", gocui.KeyArrowDown, gocui.ModNone, cursorDown); err != nil {
		return err
	}
	if err := g.SetKeybinding("side", gocui.KeyArrowUp, gocui.ModNone, cursorUp); err != nil {
		return err
	}
	if err := g.SetKeybinding("edit", gocui.KeyArrowDown, gocui.ModNone, cursorDown); err != nil {
		return err
	}
	if err := g.SetKeybinding("edit", gocui.KeyArrowUp, gocui.ModNone, cursorUp); err != nil {
		return err
	}
	if err := g.SetKeybinding("close", gocui.KeyArrowDown, gocui.ModNone, cursorDown); err != nil {
		return err
	}
	if err := g.SetKeybinding("close", gocui.KeyArrowUp, gocui.ModNone, cursorUp); err != nil {
		return err
	}
	if err := g.SetKeybinding("", gocui.KeyCtrlC, gocui.ModNone, QuitOption); err != nil {
		return err
	}
	if err := g.SetKeybinding("quit", gocui.KeyEnter, gocui.ModNone, quit); err != nil {
		return err
	}
	if err := g.SetKeybinding("", gocui.KeyCtrlL, gocui.ModNone, quit); err != nil {
		return err
	}
	if err := g.SetKeybinding("side", gocui.KeyEnter, gocui.ModNone, Display); err != nil {
		return err
	}
	if err := g.SetKeybinding("file", gocui.KeyEnter, gocui.ModNone, DisplayFile); err != nil {
		return err
	}
	if err := g.SetKeybinding("msg", gocui.KeyEnter, gocui.ModNone, delMsg); err != nil {
		return err
	}
	if err := g.SetKeybinding("close", gocui.KeyEnter, gocui.ModNone, DisplayQuit); err != nil {
		return err
	}
	if err := g.SetKeybinding("errmsg", gocui.KeyEnter, gocui.ModNone, delError); err != nil {
		return err
	}
	if err := g.SetKeybinding("clear", gocui.KeyEnter, gocui.ModNone, delBoth); err != nil {
		return err
	}
	if err := g.SetKeybinding("hostinput", gocui.KeyEnter, gocui.ModNone, EditIPaddress); err != nil {
		return err
	}
	if err := g.SetKeybinding("port", gocui.KeyEnter, gocui.ModNone, EditPort); err != nil {
		return err
	}
	if err := g.SetKeybinding("input", gocui.KeyEnter, gocui.ModNone, EditIPaddress2); err != nil {
		return err
	}		
	if err := g.SetKeybinding("portinput", gocui.KeyEnter, gocui.ModNone, EditPort2); err != nil {
		return err
	}
	return nil
	
}


func layout(g *gocui.Gui) error {


	if v, err := g.SetView("side", 0, 0, 60, 10); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		v.Title = "Display Or Edit Configuration Info Interface"
		v.Highlight = true
		fmt.Fprintln(v, "")
		fmt.Fprintln(v, "Display Host IP Address:")
		fmt.Fprintln(v, "Display Host Port Number Used:")
		fmt.Fprintln(v, "Edit Host IP Address:")
		fmt.Fprintln(v, "Edit Host Port Number:")
		fmt.Fprintln(v, "Edit Host IP Address & Port Number:")
		

	}
	if v, err := g.SetView("file", 60, 0, 120, 18); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		v.Title = "Display Configuration File"
		v.Highlight = true
		fmt.Fprintln(v, "")
		fmt.Fprintln(v, "Show Configuration File:")
		
	}
	if v, err := g.SetView("main", 0, 10, 60, 18); err != nil {
		if err != gocui.ErrUnknownView {
			return err
		}
		fmt.Fprintln(v, "KEYBINDINGS FOR INTERFACE")
		fmt.Fprintln(v, "______________________________")
		fmt.Fprintln(v, "↑ ↓: Move Cursor")
		fmt.Fprintln(v, "Enter: To Select Interface Options")
		fmt.Fprintln(v, "TAB: To Switch Views")
		fmt.Fprintln(v, "CTRL+C: Exit")

		v.Wrap = true	
		if err := g.SetCurrentView("side"); err != nil {
			return err
		}
	}
	return nil
}



func main() {
	g := gocui.NewGui()
	if err := g.Init(); err != nil {
		log.Panicln(err)
	}
	defer g.Close()

	g.SetLayout(layout)
	if err := keybindings(g); err != nil {
		log.Panicln(err)
	}
	g.SelBgColor = gocui.ColorGreen
	g.SelFgColor = gocui.ColorBlack
	g.Cursor = true

	if err := g.MainLoop(); err != nil && err != gocui.ErrQuit {
		log.Panicln(err)
	}
}