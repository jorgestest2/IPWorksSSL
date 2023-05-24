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
  global sslserver
  print(sslserver.get_remote_host(e.connection_id) + " has connected.")

def fireDisconnected(e):
  global sslserver
  print("Disconnected from %s" % sslserver.get_remote_host(e.connection_id))

def fireDataIn(e):
  global sslserver
  print("Received a message from %s and the message is: %s" % (sslserver.get_remote_host(e.connection_id), e.text.decode("utf-8")))
  print("Echoing the message back...")
  sslserver.send(e.connection_id, e.text)

def fireSSLClientAuthentication(e):
  e.accept = True

print("*********************************************************************")
print("* This is a demo to show how to setup an echo server using SSLServer*")
print("*********************************************************************")

if len(sys.argv) != 4:
  print("usage: ssl_echo_client.py port filename password")
  print("")
  print("  port    the TCP port in the local host where the component listens")
  print("  filename   the path to the file containing certificates and optional private keys")
  print("  password   the password for the certificate store file. If the provided test file is used, set the password to \"test\"")
  print("\r\nExample: ssl_echo_client.py 777 test.pfx test")
else:
  try:
    sslserver = SSLServer()
    sslserver.on_connected = fireConnected
    sslserver.on_disconnected = fireDisconnected
    sslserver.on_data_in = fireDataIn
    sslserver.on_ssl_client_authentication = fireSSLClientAuthentication

    sslserver.set_local_port(int(sys.argv[1]))

    with open(sys.argv[2], "rb") as binaryfile :
      myArr = bytearray(binaryfile.read())
    
    sslserver.set_ssl_cert_store_password(sys.argv[3])
    sslserver.set_ssl_cert_subject("*")
    sslserver.set_ssl_cert_store(myArr)

    sslserver.set_listening(True)

    print("Listening...")
    print("Press Ctrl-C to shutdown")

    while True:
      sslserver.do_events()

  except IPWorksSSLError as e:
    print("ERROR: %s" % e.message)

  except KeyboardInterrupt:
    print("Shutdown requested...exiting")
    sslserver.shutdown()


