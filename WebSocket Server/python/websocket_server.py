#
# IPWorks SSL 2022 Python Edition - Demo Application
#
# Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
#

import sys
import string
from ipworksssl import *

input = sys.hexversion<0x03000000 and raw_input or input

def fireConnected(e):
  global wsserver
  print(wsserver.get_ws_connection_remote_host(e.connection_id) + " has connected.")

def fireDisconnected(e):
  global wsserver
  print("Disconnected from %s" % wsserver.get_ws_connection_remote_host(e.connection_id))

def fireDataIn(e):
  global wsserver
  print("Received a message from %s and the message is: %s" % (wsserver.get_ws_connection_remote_host(e.connection_id), e.text))
  print("Echoing the message back...")
  wsserver.send(e.connection_id, e.text)

print("*****************************************************************\n")
print("* This demo shows how to set up an echo server using WSServer.  *\n")
print("*****************************************************************\n")

port = int(input("Local Port: "))

try:
  wsserver = WSServer()
  wsserver.on_connected = fireConnected
  wsserver.on_disconnected = fireDisconnected
  wsserver.on_data_in = fireDataIn

  buffer = ""
  while buffer == "":
    buffer = input("Use SSL? [y/n]")

  if buffer == "y" or buffer == "Y":
    buffer = ""
    while buffer == "":
      print("Please choose a certificate type:")
      print("1: PFX File")
      print("2: PEMKey File")
      buffer = input("Selection: ")

    if buffer == "1" or buffer == "": #by default use PFX
      wsserver.set_ssl_cert_store_type(2)
    else:
      wsserver.set_ssl_cert_store_type(6)

    buffer = ""
    buffer = input("Enter Certificate File Location (.\sslcert.pfx): ")
    if buffer == "":
      buffer = ".\sslcert.pfx"

    wsserver.set_ssl_cert_store(buffer)

    buffer = ""
    buffer = input("Enter Certificate Password (password): ")
    if buffer == "":
      buffer = "password"

    wsserver.set_ssl_cert_store_password(buffer)
    wsserver.set_ssl_cert_subject("*")
  
  wsserver.set_local_port(port)
  wsserver.set_local_host('localhost')
  wsserver.start_listening()
  print("Listening... press Ctrl-C to shutdown")
  while True:
    wsserver.do_events()

except IPWorksSSLError as e:
  print(e)
  print("ERROR: %s" % e.message)

except KeyboardInterrupt:
  print("Shutdown requested...exiting")
  wsserver.shutdown()

