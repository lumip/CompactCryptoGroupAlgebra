# SPDX-FileCopyrightText: 2024 Lukas Prediger <lumip@lumip.de>
# SPDX-License-Identifier: GPL-3.0-or-later

def parse_nist_param(b):
    """Given a parameter definition as specified in NIST documents, output the byte array definition.

    The mainly involves reverting bytes due to different assumed endianess.
    """
    b = b.replace(' ','').replace("\n","")
    return ' '.join(list(reversed([f"0x{b[i:i+2].lower()}," for i in range(0, len(b), 2)])))
